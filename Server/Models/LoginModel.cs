namespace Server.Models;

public class LoginModel
{
    public string Account { get; set; }
    
    public string EncryptedPassword { get; set; }
}