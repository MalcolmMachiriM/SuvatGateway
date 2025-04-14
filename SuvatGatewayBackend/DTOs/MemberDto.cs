using System;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public AppBusinessDto? AppBusiness { get; set; }
    public int AppBusinessId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
