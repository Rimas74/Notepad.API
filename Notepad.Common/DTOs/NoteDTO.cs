namespace Notepad.Common.DTOs
{
    public class NoteDTO
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
    }
}
