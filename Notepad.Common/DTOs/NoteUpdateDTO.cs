using Microsoft.AspNetCore.Http;

namespace Notepad.Common.DTOs
{
    public class NoteUpdateDTO
    {
        public int NoteId { get; set; }
        public string Title { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile Image { get; set; }
        public string Content { get; set; }
        public int? CategoryId { get; set; }
    }
}
