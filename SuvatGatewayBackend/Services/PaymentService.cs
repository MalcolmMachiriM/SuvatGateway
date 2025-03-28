using System;
using System.Text;
using System.Text.Json;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentDbContext _dbContext;
    private readonly HttpClient _httpClient;
    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        switch (request.Provider.ToLower())
        {
            case "ecocash":
                return await ProcessEcoCashPayment(request);
            case "onemoney":
                return await ProcessOneMoneyPayment(request);
            case "visa":
                return await ProcessVisaPayment(request);
            default:
                return new PaymentResponse { Success = false, Message = "Invalid provider" };
        }
    }

    
     private async Task<PaymentResponse> ProcessEcoCashPayment(PaymentRequest request)
    {
        var payload = new
        {
            phoneNumber = request.PhoneNumber,
            amount = request.Amount,
            currenc = request.Currency
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.ecocash.co.zw/payments", content);

        if (response.IsSuccessStatusCode)
        {
            return new PaymentResponse { Success = true, Message = "EcoCash payment successful" };
        }
        else
        {
            return new PaymentResponse { Success = false, Message = "EcoCash payment failed" };
        }
    }

    private async Task<PaymentResponse> ProcessOneMoneyPayment(PaymentRequest request)
    {
        // TODO: Implement real API integration
        await Task.Delay(1000);
        return new PaymentResponse { Success = true, Message = "OneMoney payment successful" };
    }

    private async Task<PaymentResponse> ProcessVisaPayment(PaymentRequest request)
    {
        // TODO: Implement real API integration
        await Task.Delay(1000);
        return new PaymentResponse { Success = true, Message = "Visa payment successful" };
    }
}
