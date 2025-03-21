using System;

namespace SuvatGatewayBackend.Entities;

public class TransactionStatus
{
    public int Id { get; set; }
    public required string Status { get; set; }
}
