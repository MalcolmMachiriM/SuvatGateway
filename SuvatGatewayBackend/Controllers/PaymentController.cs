using System;
using Microsoft.AspNetCore.Mvc;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Controllers;

public class PaymentController :BaseApiController
{
    private readonly IPaymentService _paymentService;
    private readonly DataContext _dbContext;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
        
    }

    [HttpPost("process")] 
    public async Task<IActionResult> ProcessPayment([FromBody] EcopayRequest request)
    {
        var result = await _paymentService.ProcessPaymentAsync(request);
        
        var transaction = new PaymentTransaction
        {
            Provider = request.ProvisionedService,
            PhoneNumber = request.Payer,
            Amount = request.Amount,
            Timestamp = DateTime.UtcNow
        };
        
        _dbContext.PaymentTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
        
        return Ok(result);
    }
}
