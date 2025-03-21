using System;

namespace SuvatGatewayBackend.Entities;

public class BusinessSize
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public required string Size { get; set; }

}
