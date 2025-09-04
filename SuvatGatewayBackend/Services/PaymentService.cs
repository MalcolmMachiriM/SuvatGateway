using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Helpers;
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
            case "onemoney":
                return await ProcessOneMoneyPayment(request);
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
            currency = request.Currency,
            channel = request.TransType.ToUpper()
        };

        var authContent = new StringContent(JsonSerializer.Serialize(authPayload), Encoding.UTF8, "application/json");
        httpClient.DefaultRequestHeaders.Clear();
        // httpClient.DefaultRequestHeaders.Add("X-Merchant-Key", config["_omariApiKey"]);
        httpClient.DefaultRequestHeaders.Add("X-Merchant-Key", config["_omariApiKey"]);

        var authResponse = await httpClient.PostAsync($"{config["_omariBaseUrl"]}/auth", authContent);
        if (!authResponse.IsSuccessStatusCode)
            return new PaymentResponse { Success = false, Message = "Omari auth request failed" };

        var authBody = await authResponse.Content.ReadAsStringAsync();
        using var authJson = JsonDocument.Parse(authBody);
        var otpRef = authJson.RootElement.GetProperty("otpReference").GetString();

        // Simulate OTP entry ( ndichabvisa & replace with actual capture mechanism from web)
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
        // var statusEndpoint = "https://b2bpayments.econet.co.zw/payments-service/transactions/{id}";
        var statusEndpoint = config["EcopayTrackUrl"]?.Replace("{id}", trackingNumber!);



        var maxRetries = 5;
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

private async Task<PaymentResponse> ProcessOneMoneyPayment(EcopayRequest request)
{
    string transOrderNo = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
    string random = Guid.NewGuid().ToString();

    var bizParams = new
    {
        transOrderNo,
        amt = request.Amount,
        currency = request.Currency,
        mobileNo = request.Payer,
        goodsName = "Goods/Service",
        notifyUrl = config["OneMoneyNotifyUrl"]
    };

    var bizJson = JsonSerializer.Serialize(bizParams);
    var aesKey = OneMoneyCryptoHelper.GenerateAESKey();
    var encryptData = OneMoneyCryptoHelper.EncryptWithAES(bizJson, aesKey);
    var encryptKey = OneMoneyCryptoHelper.EncryptWithRSA(aesKey, config["OneMoneyPublicKey"]);
    var signData = OneMoneyCryptoHelper.ComputeSha256Hash(bizJson);

    var payload = new
    {
        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        random,
        language = "en",
        encryptKeyId = config["OneMoneyKeyId"],
        merNo = config["OneMoneyMerchantCode"],
        encryptData,
        encryptKey,
        signData
    };

    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync($"{config["OneMoneyBaseUrl"]}/api/pay/payment/push", content);

    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadAsStringAsync();
        return new PaymentResponse { Success = false, Message = $"OneMoney request failed: {error}" };
    }

    var body = await response.Content.ReadAsStringAsync();
    using var json = JsonDocument.Parse(body);
    var status = json.RootElement.GetProperty("status").GetString();

    if (status == "0")
        return new PaymentResponse { Success = true, Message = "OneMoney push sent successfully" };

    return new PaymentResponse { Success = false, Message = $"OneMoney returned status: {status}" };
}


}
