namespace NaviConnectWebApi.Models;

public class User
{
    public User(Guid id, string city, string username, string firstName, string lastname, string surName, string avatarPath, string refreshToken)
    {
        Id = id;
        City = city;
        Username = username;
        FirstName = firstName;
        Lastname = lastname;
        SurName = surName;
        AvatarPath = avatarPath;
        RefreshToken = refreshToken;
    }

    public User()
    {
        throw new NotImplementedException();
    }

    public Guid Id { get; set; }
    public string City { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string Lastname { get; set; }
    public string SurName { get; set; }
    // // public string Phone { get; set; }
    public string AvatarPath { get; set; }
    public byte[] PasswordHash { get; set; }  = new byte[32];
    public byte[] PasswordSalt { get; set; }  = new byte[32];
    public string RefreshToken { get; set; }
}