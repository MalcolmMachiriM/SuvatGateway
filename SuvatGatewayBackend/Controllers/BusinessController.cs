using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Controllers;

public class BusinessController(DataContext context) : BaseApiController
{
    [Authorize]
    [HttpPost("register/business")]
    public async Task<ActionResult<AppBusiness>> RegisterBusiness(BusinessDto businessDto)
    {
        if (await BusinessExists(businessDto.Name)) return BadRequest("Business Exists");
        
        var business = new AppBusiness
        {
            Name =  businessDto.Name,
            Email = businessDto.Email,
            PhoneNumber = businessDto.PhoneNumber,
            Description = businessDto.Description,
            RegistrationNumber = businessDto.RegistrationNumber,
            RegistrationType = businessDto.RegistrationType,
            Address = businessDto.Address,
            Website = businessDto.Website
        };

        context.Businesses.Add(business);
        await context.SaveChangesAsync();

        return business;
    }

    [HttpGet("business/exists/{name}")]
    public async Task<bool> BusinessExists(string name)
    {
        return await context.Businesses.AnyAsync(x=> x.Name.ToLower() == name.ToLower());
    }
}
