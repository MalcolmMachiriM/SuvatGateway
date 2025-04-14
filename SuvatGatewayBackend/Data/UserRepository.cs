using System;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.DTOs;
using SuvatGatewayBackend.Entities;
using SuvatGatewayBackend.Interfaces;

namespace SuvatGatewayBackend.Data;

public class UserRepository(DataContext context) : IUserRepository
{
    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await context.Users.SingleOrDefaultAsync(x => x.UserName == username);
    }

    public Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        throw new NotImplementedException();
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
