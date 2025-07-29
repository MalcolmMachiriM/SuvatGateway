using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Services;

public class PaymentService(IConfiguration config, HttpClient httpClient, DataContext context) : IPaymentService
{

    public async Task<PaymentResponse> ProcessPaymentAsync(EcopayRequest request)
    {
        switch (request.ProvisionedService.ToLower())
        {
            case "ecocash":
                return await ProcessEcoCashPayment(request);
             case "omari":
                 return await ProcessOmariPayment(request);
            // case "visa":
            //     return await ProcessVisaPayment(request);
            default:
                return new PaymentResponse { Success = false, Message = "Invalid provider" };
        }
    }

    private async Task<PaymentResponse> ProcessOmariPayment(EcopayRequest request)
    {
        var reference = Guid.NewGuid().ToString();

    var authPayload = new
    {
        msisdn = request.Payer,
        reference = reference,
        amount = request.Amount,
        currency = "USD",
        channel = "WEB"
    };

    var authContent = new StringContent(JsonSerializer.Serialize(authPayload), Encoding.UTF8, "application/json");
    httpClient.DefaultRequestHeaders.Clear();
    httpClient.DefaultRequestHeaders.Add("X-Merchant-Key", config["_omariApiKey"]);

    var authResponse = await httpClient.PostAsync($"{config["_omariBaseUrl"]}/auth", authContent);
    if (!authResponse.IsSuccessStatusCode)
        return new PaymentResponse { Success = false, Message = "Omari auth request failed" };

    var authBody = await authResponse.Content.ReadAsStringAsync();
    using var authJson = JsonDocument.Parse(authBody);
    var otpRef = authJson.RootElement.GetProperty("otpReference").GetString();

    // Simulate OTP entry (replace with actual capture mechanism)
     // Display OTP reference to the user and wait for input
    Console.WriteLine($"Please enter the OTP sent to {request.Payer}. Reference: {otpRef}");
    Console.Write("Enter OTP: ");
    string? enteredOtp = Console.ReadLine();

    var payPayload = new
    {
        msisdn = request.Payer,
        reference = reference,
        otp = enteredOtp
    };


    var payContent = new StringContent(JsonSerializer.Serialize(payPayload), Encoding.UTF8, "application/json");
    var payResponse = await httpClient.PostAsync($"{config["_omariBaseUrl"]}/request", payContent);
    if (!payResponse.IsSuccessStatusCode)
        return new PaymentResponse { Success = false, Message = "Omari payment request failed" };

    var payBody = await payResponse.Content.ReadAsStringAsync();
    using var payJson = JsonDocument.Parse(payBody);
    var statusCode = payJson.RootElement.GetProperty("responseCode").GetString();

    if (statusCode == "000")
    {
        // Query final status from Omari
        return await QueryOmariPaymentStatus(reference);
    }

    return new PaymentResponse { Success = false, Message = "Omari payment failed" };
    }

    private async Task<PaymentResponse> QueryOmariPaymentStatus(string reference)
{
    var queryUrl = $"{config["_omariBaseUrl"]}/query/{reference}";
    var request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
    request.Headers.Add("X-Merchant-Key", config["_omariApiKey"]);

    var response = await httpClient.SendAsync(request);
    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadAsStringAsync();
        return new PaymentResponse { Success = false, Message = $"Omari status check failed: {error}" };
    }

    var content = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(content);
    var status = doc.RootElement.GetProperty("status").GetString();

    return status == "Success"
        ? new PaymentResponse { Success = true, Message = "Omari payment confirmed" }
        : new PaymentResponse { Success = false, Message = $"Omari payment status: {status}" };
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
        var delay = 2000; // 5 seconds
        for (int i = 0; i < maxRetries; i++)
        {
            await Task.Delay(delay);
            //var statusResponse = await httpClient.GetAsync(config["EcopayTrackUrl"]);
            var statusRequest = new HttpRequestMessage(HttpMethod.Get, statusEndpoint);
            statusRequest.Headers.Authorization = new AuthenticationHeaderValue("Authorization", $"Bearer {config["EcopayApiToken"]}");
            var statusResponse = await httpClient.SendAsync(statusRequest);


            var statusContent = await statusResponse.Content.ReadAsStringAsync();
            using var statusDoc = JsonDocument.Parse(statusContent);
            var status = statusDoc.RootElement.GetProperty("transactionStatus").GetString(); // e.g., "SUCCESS", "PENDING", "FAILED"

            if (status == "SUCCESS")
                return new PaymentResponse { Success = true, Message = "EcoCash payment confirmed" };
            if (status == "FAILED")
                return new PaymentResponse { Success = false, Message = "EcoCash payment failed" };

        }
        var transaction = new PaymentTransaction
        {
            Provider = request.ProvisionedService,
            PhoneNumber = request.Payer,
            Amount = request.Amount,
            Timestamp = DateTime.UtcNow,
            Status = "status",
            TrackingNumber = trackingNumber!
        };

        context.PaymentTransactions.Add(transaction);
        await context.SaveChangesAsync();

        return new PaymentResponse { Success = false, Message = "EcoCash payment not confirmed in time" };
    }



}
