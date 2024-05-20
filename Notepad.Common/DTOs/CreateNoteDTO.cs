using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Common.DTOs
{
    public class CreateNoteDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile Image { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
