using System;

namespace SuvatGatewayBackend.Entities;

public class AppBusiness
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public int SizeId { get; set; }
    public required string Email { get; set; }
    public required string Description { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Website { get; set; }
    public required string RegistrationNumber { get; set; }
    public required string RegistrationType { get; set; }
    public required string Address { get; set; }
}
