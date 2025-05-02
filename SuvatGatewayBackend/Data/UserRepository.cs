using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
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

    public Task<AppUser?> GetUserByUserName(string username)
    {
        throw new NotImplementedException();
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
