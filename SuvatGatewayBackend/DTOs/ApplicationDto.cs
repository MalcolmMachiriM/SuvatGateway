using System;

namespace SuvatGatewayBackend.DTOs;

public class ApplicationDto
{
    public required string Name { get; set; }
    public int TransactionFeeChargeType { get; set; }
    public string? Description { get; set; }
    public bool PaymentPage { get; set; }
    public int BusinessId { get; set; }
}
