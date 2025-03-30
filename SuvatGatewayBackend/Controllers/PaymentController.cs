using System;
using Microsoft.AspNetCore.Mvc;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Controllers;

public class PaymentController(DataContext context, IPaymentService paymentService) :BaseApiController
{
    
    [HttpPost("process")] 
    public async Task<ActionResult> ProcessPayment([FromBody] EcopayRequest request)
    {
        var result = await paymentService.ProcessPaymentAsync(request);
        
        var transaction = new PaymentTransaction
        {
            Provider = request.ProvisionedService,
            PhoneNumber = request.Payer,
            Amount = request.Amount,
            Timestamp = DateTime.UtcNow
        };
        
        context.PaymentTransactions.Add(transaction);
        await context.SaveChangesAsync();
        
        return Ok(result);
    }
}
