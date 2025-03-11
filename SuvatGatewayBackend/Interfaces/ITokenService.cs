using System;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
    
}
