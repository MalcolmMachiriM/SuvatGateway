using System;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Data;

public class UserRepository(DataContext context, IMapper mapper, ITokenService tokenService) : IUserRepository
{
    public async Task<UserDto?> AddMemberAsync(RegisterDto registerDto )
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();

        var user = new AppUser
        {
            Firstname = registerDto.Firstname,
            Lastname = registerDto.Lastname,
            UserName = registerDto.Username.ToLower(),
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)

        }
        ;
    }

    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await context.Users
        .Where(x=> x.UserName == username)
        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await context.Users
        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<IEnumerable<AppUser>> GetUserAsync()
    {
        return await context.Users
        // .Include(x => x.PasswordHash)  //how to include stuff
        .ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id) ;
    }

    public async Task<AppUser?> GetUserByUserName(string username)
    {
        return await context.Users.SingleOrDefaultAsync(x=> x.UserName == username ) ;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
