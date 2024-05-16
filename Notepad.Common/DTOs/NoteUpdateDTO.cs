using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Notepad.Common.DTOs
{
    public class NoteUpdateDTO
    {
        //public int NoteId { get; set; }
        [Required]
        public string Title { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile Image { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int? CategoryId { get; set; }
    }
}
