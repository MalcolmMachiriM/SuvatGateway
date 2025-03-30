using System;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(EcopayRequest request);
}
