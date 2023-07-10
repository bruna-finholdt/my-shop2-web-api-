using Microsoft.EntityFrameworkCore;
using Minishop.DAL;
using Minishop.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddTransient<MiniShopServices>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<Minishop2023Context>(options => options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader());
});

var healthChecks = builder.Services.AddHealthChecks();
if (connectionString != null)
    healthChecks.AddSqlServer(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Minishop2023Context>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("CorsPolicy");
}

//health check endpoint
app.MapHealthChecks("/health");

app.UseAuthorization();

app.MapControllers();

app.Run();