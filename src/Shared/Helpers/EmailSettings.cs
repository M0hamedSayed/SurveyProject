namespace Shared.Helpers
{
    public class EmailSettings
    {
        public int Port { get; set; }
        public string Host { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
