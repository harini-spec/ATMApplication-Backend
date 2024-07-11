using ATMApplication.Models;
using ATMApplication.Repositories;
using ATMApplication.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ATMApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ATMContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

            // Register repositories
            builder.Services.AddScoped<IRepository<int, Card>, CardRepository>();
            builder.Services.AddScoped<IRepository<int, Account>, AccountRepository>();
            builder.Services.AddScoped<IRepository<int, Customer>, CustomerRepository>();
            builder.Services.AddScoped<IRepository<Guid, Transaction>, TransactionRepository>();


            // Register services
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Configure Swagger/OpenAPI
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ATM Application API", Version = "v1" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ATM Application API V1");
                });
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
