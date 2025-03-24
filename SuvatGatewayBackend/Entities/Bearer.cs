using System;

namespace SuvatGatewayBackend.Entities;

public class Bearer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
