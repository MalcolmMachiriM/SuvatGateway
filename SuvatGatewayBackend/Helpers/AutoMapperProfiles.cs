using System;
using AutoMapper;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Helpers;

public class AutoMapperProfiles : Profile
{
    AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>();
        CreateMap<AppUser, RegisterDto>();
        CreateMap<UserDto, RegisterDto>();
        CreateMap<AppBusiness, AppBusinessDto>();

    }
}
