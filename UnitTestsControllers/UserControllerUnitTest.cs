using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Notepad.API.Controllers;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using Notepad.Repositories.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class UserControllerUnitTest : IAsyncLifetime
{
    private UserController _userController;
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<SignInManager<User>> _signInManagerMock;
    private Mock<ITokenService> _tokenServiceMock;

    private LoginDTO _loginDTO;
    private RegisterDTO _registerDTO;
    private User _user;
    private TokenResponseDTO _tokenResponseDTO;

    public async Task InitializeAsync()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
        _tokenServiceMock = new Mock<ITokenService>();

        _loginDTO = new LoginDTO { Username = "testuser", Password = "Password123!" };
        _registerDTO = new RegisterDTO { Username = "testuser", Email = "test@example.com", Password = "Password123!" };
        _user = new User { UserName = _registerDTO.Username, Email = _registerDTO.Email };
        _tokenResponseDTO = new TokenResponseDTO { Token = "sampletoken" };

        _userManagerMock.Setup(um => um.FindByNameAsync(_loginDTO.Username)).ReturnsAsync(_user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(_user, _loginDTO.Password)).ReturnsAsync(true);
        _tokenServiceMock.Setup(ts => ts.GenerateToken(_user)).ReturnsAsync(_tokenResponseDTO.Token);

        _userController = new UserController(_userManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object);
    }

    public Task DisposeAsync()
    {
        // Clean up any resources if needed
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WithValidCredentials()
    {
        // Act
        var result = await _userController.Login(_loginDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TokenResponseDTO>(okResult.Value);
        Assert.Equal(_tokenResponseDTO.Token, returnValue.Token);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WithInvalidCredentials()
    {
        // Arrange
        _userManagerMock.Setup(um => um.CheckPasswordAsync(_user, _loginDTO.Password)).ReturnsAsync(false);

        // Act
        var result = await _userController.Login(_loginDTO);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WithValidData()
    {
        // Arrange
        var identityResult = IdentityResult.Success;
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(identityResult);

        // Act
        var result = await _userController.Register(_registerDTO);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WithInvalidData()
    {
        // Arrange
        var identityErrors = new List<IdentityError>
        {
            new IdentityError { Description = "Invalid data" }
        };
        var identityResult = IdentityResult.Failed(identityErrors.ToArray());
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(identityResult);

        // Act
        var result = await _userController.Register(_registerDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorList = Assert.IsType<List<IdentityError>>(badRequestResult.Value);
        Assert.Contains(errorList, e => e.Description == "Invalid data");
    }
}
