using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Services;

public class PaymentService(IConfiguration config, HttpClient httpClient) : IPaymentService
{

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
            transType = "PAYIN",
            currency = request.Currency,
            merchantCode = config["MerchantCode"],
            provisionedService = "ecocash"
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["EcopayApiToken"]}");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var initialResponse = await httpClient.PostAsync(config["EcopayUrl"], content);

        if (!initialResponse.IsSuccessStatusCode)
        {
            var error = await initialResponse.Content.ReadAsStringAsync();
            return new PaymentResponse { Success = false, Message = $"Initial EcoCash request failed: {error}" };
        }

        var responseBody = await initialResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseBody);
        var trackingNumber = doc.RootElement.GetProperty("trackingNumber").GetString();

        // Poll for payment confirmation
        var statusEndpoint = "https://b2bpayments.econet.co.zw/payments-service/transactions/{id}";



        var maxRetries = 10;
        var delay = 5000; // 5 seconds
        for (int i = 0; i < maxRetries; i++)
        {
            await Task.Delay(delay);
            //var statusResponse = await httpClient.GetAsync(config["EcopayTrackUrl"]);
            var statusRequest = new HttpRequestMessage(HttpMethod.Get, statusEndpoint);
            statusRequest.Headers.Authorization = new AuthenticationHeaderValue("Authorization", $"Bearer {config["EcopayApiToken"]}");
            var statusResponse = await httpClient.SendAsync(statusRequest);

            if (statusResponse.IsSuccessStatusCode)
            {
                var statusContent = await statusResponse.Content.ReadAsStringAsync();
                using var statusDoc = JsonDocument.Parse(statusContent);
                var status = statusDoc.RootElement.GetProperty("transactionStatus").GetString(); // e.g., "SUCCESS", "PENDING", "FAILED"

                if (status == "SUCCESS")
                    return new PaymentResponse { Success = true, Message = "EcoCash payment confirmed" };
                if (status == "FAILED")
                    return new PaymentResponse { Success = false, Message = "EcoCash payment failed" };
            }
        }

        return new PaymentResponse { Success = false, Message = "EcoCash payment not confirmed in time" };
    }


}
