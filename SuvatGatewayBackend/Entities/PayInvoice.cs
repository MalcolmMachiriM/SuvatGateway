using System;

namespace SuvatGatewayBackend.Entities;

public class PayInvoice
{
    public int Id { get; set; }
    public required string Narative { get; set; }
    public float Amount { get; set; }
    public int CurrencyId { get; set; }
    public DateOnly ProcessingDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public DateOnly DueDate { get; set; }
    public bool Reccuring { get; set; }
    public int BusinessId { get; set; }
    public required string PayerEmail { get; set; }
    public string? Name { get; set; }
    public required string PhoneNumber { get; set; }


}
