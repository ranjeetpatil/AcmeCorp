using AcmeCorp.Core.Data;
using AcmeCorp.Core.Handlers;
using AcmeCorp.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .AddJsonFile("appsettings." + builder.Environment.EnvironmentName.ToUpper() + ".json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AcmeCorpDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "APIKey";
    options.DefaultChallengeScheme = "APIKey";
}).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("APIKey", options => { });

//IApiKeyService implementation
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
