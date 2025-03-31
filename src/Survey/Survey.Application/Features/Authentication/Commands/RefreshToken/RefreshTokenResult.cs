namespace Survey.Application.Features.Authentication.Commands.RefreshToken
{
    public record RefreshTokenResult
    {
        public required string Token { get; set; }
    }
}
