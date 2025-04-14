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
        CreateMap<AppBusiness, AppBusinessDto>();
    }
}
