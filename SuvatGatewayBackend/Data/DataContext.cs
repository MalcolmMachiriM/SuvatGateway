using System;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Entities;

namespace SuvatGatewayBackend.Data;

public class DataContext(DbContextOptions options): DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<AppBusiness> Businesses {get; set;}
    public DbSet<Application> Applications { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<PayInvoice> PayInvoices { get; set; }
    public DbSet<BusinessCategory> BusinessCategories { get; set; }
    public DbSet<BusinessSize> BusinessSizes { get; set; }
    public DbSet<TransactionFeeChargeType> TransactionFeeChargeTypes { get; set; }
    public DbSet<InvPayer> InvPayers { get; set; }
    public DbSet<TransactionStatus> TransactionStatuses { get; set; }
    public DbSet<PayTransaction> PayTransactions { get; set; }
}
