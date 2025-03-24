using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Controllers;

[Authorize]
public class InvoicesController(DataContext context) : BaseApiController
{
    [HttpPost("create/invoice")]
    public async Task<ActionResult<PayInvoice>> AddInvoice(InvoiceDto invoiceDto)
    {
        if (await InvoiceExists(invoiceDto.Narative, invoiceDto.Amount)) return BadRequest("Application exists");

        var invoice = new PayInvoice
        {
            Name = invoiceDto.Name,
            Narative = invoiceDto.Narative,
            Amount = invoiceDto.Amount,
            CurrencyId = invoiceDto.CurrencyId,
            ProcessingDate = invoiceDto.ProcessingDate,
            DueDate= invoiceDto.DueDate,
            Reccuring= invoiceDto.Reccuring,
            BusinessId= invoiceDto.BusinessId,
            PayerEmail= invoiceDto.PayerEmail,
            PhoneNumber= invoiceDto.PhoneNumber,
            CreatedDate= DateTime.Now

        };

        context.PayInvoices.Add(invoice);
        await context.SaveChangesAsync();

        return invoice;
    }

    [HttpGet("invoice/exists/{narative=string, amount:float}")]
    public async Task<bool> InvoiceExists(string narative, float amount)
    {
        return await context.PayInvoices.AnyAsync(x => x.Narative.ToLower() == narative.ToLower() && x.Amount == amount);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PayInvoice>>> GetInvoices()
    {
        var invoices = await context.PayInvoices.ToListAsync();

        return invoices;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PayInvoice>> GetInvoice(int id)
    {
        var invoice = await context.PayInvoices.FindAsync(id);

        if (invoice == null) return NotFound();

        return invoice;
    }

}
