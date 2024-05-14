namespace Notepad.Common.DTOs
{
    public class NoteUpdateDTO
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? CategoryId { get; set; }
    }
}
