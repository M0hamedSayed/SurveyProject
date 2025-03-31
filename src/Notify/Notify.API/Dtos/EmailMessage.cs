namespace Notify.API.Dtos
{
    public class EmailMessage
    {
        public List<string> To { get; set; } = new();
        public List<string>? Cc { get; set; } = new();
        public List<string>? Bcc { get; set; } = new();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public List<IFormFile>? Attachments { get; set; }
    }
}
