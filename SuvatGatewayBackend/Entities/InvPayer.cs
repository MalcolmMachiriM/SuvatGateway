using System;

namespace SuvatGatewayBackend.Entities;

public class InvPayer
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? Name  { get; set; }
    public string? PhoneNumber { get; set; }
    public int BusinessId { get; set; }
}
