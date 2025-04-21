
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entities;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    public string? Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired  => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public Guid? UserId { get; set; }
}