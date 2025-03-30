using System;

namespace SuvatGatewayBackend.Entities;

public class EcopayRequest
{
    public decimal Amount { get; set; }
    public required string Payer { get; set; }
    public required string TransType { get; set; }
    public required string Currency { get; set; }
    public required string MerchantCode { get; set; }
    public required string ProvisionedService { get; set; }
}
