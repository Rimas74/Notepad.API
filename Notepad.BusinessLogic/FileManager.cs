using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Notepad.BusinessLogic
{
    public class FileManager : IFileManager
    {
        private readonly string _basePath;
        private readonly ILogger _logger;

        public FileManager(string basePath, ILogger<FileManager> logger)
        {
            _basePath = basePath; //Path.Combine(basePath, "images");
            _logger = logger;
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
                _logger.LogInformation("Created directory at {_basePath}", _basePath);
            }

        }
        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_basePath, fileName);

            _logger.LogInformation("Saving image to {filePath}", filePath);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }


    }
}
