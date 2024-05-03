using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.ApplicationDBContext;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("NOS.Engineering.Challenge.API")));

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();
    
app.Run();