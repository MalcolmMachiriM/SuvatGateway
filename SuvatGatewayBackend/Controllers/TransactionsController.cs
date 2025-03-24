using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Controllers;

[Authorize]
public class TransactionsController(DataContext context): BaseApiController
{
    [HttpPost("create/transaction")]
    public async Task<ActionResult<PayTransaction>> AddInvoice(TransactionDto transactionDto)
    {
        if (await TransactionExists(transactionDto.ReferenceNumber, transactionDto.Amount)) return BadRequest("Application exists");

        var invoice = new PayTransaction
        {
            ReferenceNumber = transactionDto.ReferenceNumber,
            TransactionDate = transactionDto.TransactionDate,
            Amount = transactionDto.Amount,
            Status = transactionDto.Status,
            ApplicationId= transactionDto.ApplicationId,
            BusinessId= transactionDto.BusinessId,
            PaymentMethodId= transactionDto.PaymentMethodId,
            TransactionFee= transactionDto.TransactionFee,
            CreatedDate= DateTime.Now

        };

        context.PayTransactions.Add(invoice);
        await context.SaveChangesAsync();

        return invoice;
    }

    [HttpGet("transaction/exists/{referenceNumber=string, amount:float}")]
    public async Task<bool> TransactionExists(string referenceNumber, float amount)
    {
        return await context.PayTransactions.AnyAsync(x => x.ReferenceNumber.ToLower() == referenceNumber.ToLower() && x.Amount == amount);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PayTransaction>>> GetTransactions()
    {
        var invoices = await context.PayTransactions.ToListAsync();

        return invoices;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PayTransaction>> GetTransactions(int id)
    {
        var transaction = await context.PayTransactions.FindAsync(id);

        if (transaction == null) return NotFound();

        return transaction;
    }
}
