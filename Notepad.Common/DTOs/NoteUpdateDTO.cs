using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Notepad.Common.DTOs
{
    public class NoteUpdateDTO
    {
        //public int NoteId { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        [Required]
        public int? CategoryId { get; set; }
    }
}
