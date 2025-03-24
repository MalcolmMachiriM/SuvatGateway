using System;

namespace SuvatGatewayBackend.DTOs;

public class TransactionDto
{
    public required string ReferenceNumber { get; set; }
    public float Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public int Status { get; set; }
    public int BusinessId { get; set; }
    public int ApplicationId { get; set; }
    public int PaymentMethodId { get; set; }
    public float TransactionFee { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
