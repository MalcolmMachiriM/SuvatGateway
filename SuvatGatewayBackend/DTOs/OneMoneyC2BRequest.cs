using System;

namespace SuvatGatewayBackend.DTOs;

public class OneMoneyC2BRequest
{
    public string? TransOrderNo { get; set; }
    public decimal Amt { get; set; }
    public string Currency { get; set; } = "ZWG";
    public string? MobileNo { get; set; }
    public string GoodsName { get; set; } = "Airtime Purchase";
    public string? NotifyUrl { get; set; }
}
