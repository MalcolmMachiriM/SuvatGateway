using System;
using System.ComponentModel.DataAnnotations;

namespace SuvatGatewayBackend.DTOs;

public class RegisterDto
{
    [Required]
    public required string Firstname { get; set; }
    [Required]
    public required string Lastname { get; set; }
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string PhoneNumber { get; set; }
    [Required]
    public required string Password { get; set; }
}
