namespace Survey.Application.Features.Authentication.Commands.Login
{
    public record LoginResult
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }

        public required ActiveSession activeSession { get; set; }
    }

    public record ActiveSession
    {
        // user metadata
        public required string AccessToken { get; set; }
        public required string UserAgent { get; set; }
        public string? IP { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? City { get; set; }
        public string? TimeZone { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLng { get; set; }
    }
}
