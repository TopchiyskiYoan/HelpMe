namespace HelpMe.Application.DTOs;

public class AuthResult
{
    public bool Succeeded { get; init; }
    public AuthResponseDto? Data { get; init; }
    public string? ErrorCode { get; init; }

    public static AuthResult Ok(AuthResponseDto data) =>
        new() { Succeeded = true, Data = data };

    public static AuthResult Fail(string errorCode) =>
        new() { Succeeded = false, ErrorCode = errorCode };
}
