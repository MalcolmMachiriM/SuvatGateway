using System;

namespace SuvatGatewayBackend.Entities;

public class TransactionFeeChargeType
{
    public int Id { get; set; }
    public required string Type { get; set; }
}
