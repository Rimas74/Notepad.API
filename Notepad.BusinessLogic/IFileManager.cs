using Microsoft.AspNetCore.Http;

namespace Notepad.BusinessLogic
{
    public interface IFileManager
    {

        Task<string> SaveImageAsync(IFormFile file);
        Task DeleteImageAsync(string imagePath);
    }
}