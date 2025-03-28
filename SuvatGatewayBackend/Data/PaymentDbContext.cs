using System;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
}
