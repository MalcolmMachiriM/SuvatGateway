using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Controllers;

public class ApplicationsController(DataContext context): BaseApiController
{
    [Authorize]
    [HttpPost("register/business")]
    public async Task<ActionResult<Application>> AddApplication(ApplicationDto applicationDto)
    {
        if (await ApplicationExists(applicationDto.Name)) return BadRequest("Application exists");

        var app = new Application
        {
            Name = applicationDto.Name,
            TransactionFeeChargeType = applicationDto.TransactionFeeChargeType,
            Description = applicationDto.Description,
            PaymentPage = applicationDto.PaymentPage
            
        };

        context.Applications.Add(app);
        await context.SaveChangesAsync();

        return app;
    }

    [HttpGet("business/exists/{name}")]
    public async Task<bool> ApplicationExists(string name)
    {
        return await context.Applications.AnyAsync(x=> x.Name.ToLower() == name.ToLower());
    }

}
