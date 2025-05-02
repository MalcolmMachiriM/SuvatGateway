using System;

namespace SuvatGatewayBackend.Entities;

public class AppUser
{
    public int Id { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int AppBusinessId { get; set; }
    public AppBusiness AppBusiness { get; set; } =null!;
}
