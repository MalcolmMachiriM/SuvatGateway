using System;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Data;

public class DataContext(DbContextOptions options): DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<AppBusiness> Businesses {get; set;}
    public DbSet<Application> Applications { get; set; }
}
