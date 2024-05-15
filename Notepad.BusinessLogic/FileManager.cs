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
            _basePath = Path.Combine(basePath, "images");
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

        }
        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_basePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }

        public bool IsPathValid(string path)
        {
            var fullPath = Path.GetFullPath(path);
            return fullPath.StartsWith(_basePath) && File.Exists(fullPath);
        }
        public byte[] GetImage(string filePath)
        {
            if (!IsPathValid(filePath))
            {
                throw new FileNotFoundException("The file path is not valid or the file does not exist.");
            }

            return File.ReadAllBytes(filePath);
        }
    }
}
