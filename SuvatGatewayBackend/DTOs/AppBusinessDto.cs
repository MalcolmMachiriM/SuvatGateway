namespace SuvatGatewayBackend.DTOs;

public class AppBusinessDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int CategoryId { get; set; }
    public int SizeId { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? RegistrationType { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public List<MemberDto>? Users { get; set; } 
    public List<ApplicationDto>? Applications {get; set;}
}