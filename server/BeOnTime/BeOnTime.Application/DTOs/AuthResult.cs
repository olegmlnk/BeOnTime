namespace BeOnTime.Application.DTOs;

public class AuthResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    
    public TokenResponseDto? Token { get; set; }
    
    public static AuthResult Ok(TokenResponseDto token) =>
    new() { Success = true, Token = token };

    public static AuthResult Fail(string error) =>
        new() { Success = false, Error = error };
}