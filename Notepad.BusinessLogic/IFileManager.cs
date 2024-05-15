using Microsoft.AspNetCore.Http;

namespace Notepad.BusinessLogic
{
    public interface IFileManager
    {
        //byte[] GetImage(string filePath);
        //bool IsPathValid(string path);
        Task<string> SaveImageAsync(IFormFile file);
    }
}