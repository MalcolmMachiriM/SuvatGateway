using System;

namespace SuvatGatewayBackend.Entities;

public class Application
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int TransactionFeeChargeType { get; set; }
    public string? Description { get; set; }
    public bool PaymentPage { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int AppBusinessId { get; set; }
    public AppBusiness AppBusiness { get; set; } = null!;
}
