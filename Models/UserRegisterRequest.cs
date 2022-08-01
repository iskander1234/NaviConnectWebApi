using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NaviConnectWebApi.Models;

public class UserRegisterRequest
{
    public UserRegisterRequest(string city, string username, string firstName, string lastname, string surName, IFormFile file, string password, string confirmPassword)
    {
        City = city;
        Username = username;
        FirstName = firstName;
        Lastname = lastname;
        SurName = surName;
        File = file;
        Password = password;
        ConfirmPassword = confirmPassword;
    }

    [Required] public string City { get; set; } 
    [Required] 
    public string Username { get; set; } 

    [Required] public string FirstName { get; set; }

    [Required]
    public string Lastname { get; set; } 

    public string SurName { get; set; } 
    
    // public string Phone { get; set; }
    
    [Required]
   
    [DataType(DataType.Upload)]
    public IFormFile File { get; set; } 
    
    [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters, dude!")]
    public string Password { get; set; }
    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; } 
}