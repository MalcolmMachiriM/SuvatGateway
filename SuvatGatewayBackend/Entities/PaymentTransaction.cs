using System;

namespace SuvatGatewayBackend.Entities;

public class PaymentTransaction
{
    public int Id { get; set; }
    public string Provider { get; set; }
    public string PhoneNumber { get; set; }
    public string CardNumber { get; set; }
    public decimal Amount { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}
