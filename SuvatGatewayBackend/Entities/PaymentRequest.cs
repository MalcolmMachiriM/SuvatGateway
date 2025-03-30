using System;

namespace SuvatGatewayBackend.Entities;

public class PaymentRequest
{
    public string? Provider { get; set; } // "ecocash", "onemoney", "visa"
    public string? PhoneNumber { get; set; } // Required for mobile money
    public string? CardNumber { get; set; } // Required for Visa
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
}
