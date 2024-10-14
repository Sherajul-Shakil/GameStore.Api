// Dtos/LoginDto.cs 
namespace GameStore.Api.Dtos;

public class LoginDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}