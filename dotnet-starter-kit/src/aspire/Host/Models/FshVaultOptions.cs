namespace FSH.Starter.Aspire.Models;

public class FshVaultOptions
{
    public string Address { get; set; } = "http://localhost:8200";
    public string Token { get; set; } = string.Empty;
    public string SecretPath { get; set; } = string.Empty;
    public string UsernameKey { get; set; } = "username";
    public string PasswordKey { get; set; } = "password";
}
