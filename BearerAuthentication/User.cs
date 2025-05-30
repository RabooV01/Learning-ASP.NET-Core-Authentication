using System.ComponentModel.DataAnnotations;

namespace BearerAuthentication;

public class User 
{
    public Guid Id { get; set; } = Guid.Empty;
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}