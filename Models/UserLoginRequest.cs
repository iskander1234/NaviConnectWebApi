using System.ComponentModel.DataAnnotations;

namespace NaviConnectWebApi.Models;

public class UserLoginRequest
{
    
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}