namespace BBShop.DTOs;

public class UserCreateDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }  // Add this line
    public string Role { get; set; }
}