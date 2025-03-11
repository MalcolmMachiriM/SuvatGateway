using System;
using Microsoft.EntityFrameworkCore;
using SuvatGatewayBackend.Data;
using SuvatGatewayBackend.Interfaces;
using SuvatGatewayBackend.Services;

namespace SuvatGatewayBackend.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();

        services.AddScoped<ITokenService, TokenService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
