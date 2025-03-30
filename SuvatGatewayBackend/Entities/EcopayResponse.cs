using System;

namespace SuvatGatewayBackend.Entities;

public class EcopayResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string TransType { get; set; }
    public required string TrackingNumber { get; set; }
    public required string TransactionStatus { get; set; }
    public required string ProvisionedService { get; set; }
    public required string Payee { get; set; }
    public required string Payer { get; set; }
    public required string MerchantCode { get; set; }
    public required string Message { get; set; }
    public required string CreationDate { get; set; }
    public required string LastModifiedDate { get; set; }
}

