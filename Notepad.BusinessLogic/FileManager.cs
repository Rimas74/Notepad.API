using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Notepad.BusinessLogic
{
    public class FileManager : IFileManager
    {
        private readonly string _basePath;

        public FileManager(string basePath)
        {
            _basePath = basePath; //Path.Combine(basePath, "images");
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

        }
        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(file.FileName)}"; 
            var filePath = Path.Combine(_basePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }


    }
}
