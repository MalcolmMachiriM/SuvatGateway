using System;
using System.Text;
using System.Text.Json;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Services;

public class PaymentService( IConfiguration config) : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
        public async Task<PaymentResponse> ProcessPaymentAsync(EcopayRequest request)
    {
        switch (request.ProvisionedService.ToLower())
        {
            case "ecocash":
                return await ProcessEcoCashPayment(request);
            // case "onemoney":
            //     return await ProcessOneMoneyPayment(request);
            // case "visa":
            //     return await ProcessVisaPayment(request);
            default:
                return new PaymentResponse { Success = false, Message = "Invalid provider" };
        }
    }

    
     private async Task<PaymentResponse> ProcessEcoCashPayment(EcopayRequest request)
    {
        var payload = new
        {
            amount = request.Amount,
            payer = request.Payer,
            transType = "payment",
            currency = request.Currency,
            merchantCode = "052736", 
            provisionedService = "Ecocash" 
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["EcopayApiToken"]}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var response = await _httpClient.PostAsync(config["EcopayUrl"], content);
        
        if (response.IsSuccessStatusCode)
        {
            return new PaymentResponse { Success = true, Message = "EcoCash payment successful" };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new PaymentResponse { Success = false, Message = $"EcoCash payment failed: {errorMessage}" };
        }
    }

    
}
