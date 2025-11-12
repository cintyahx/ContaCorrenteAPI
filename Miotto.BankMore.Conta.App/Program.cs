using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Miotto.BankMore.Conta.App.Commands;
using Miotto.BankMore.Conta.App.Configurations;
using Miotto.BankMore.Conta.App.Services;
using Miotto.BankMore.Conta.App.Validations;
using Miotto.BankMore.Conta.Domain.Interfaces;
using Miotto.BankMore.Conta.Infra;
using Miotto.BankMore.Conta.Infra.Repositories;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Conta Corrente API"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
                        "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +
                        "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +
                        "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc();

#region FluentValidation + Behavior
builder.Services.AddValidatorsFromAssemblyContaining<CreateContaCorrenteValidation>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
#endregion

#region Services

builder.Services.AddScoped<ContextMiddleware>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();

#endregion

builder.Services.AddDbContext<BankMoreContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("BankMoreContextConnection")));
Console.WriteLine(builder.Configuration.GetConnectionString("BankMoreContextConnection"));

#region AuthorizationConfigs

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("Key");

if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception(jwtSettings.Value);
}

builder.Services.AddSingleton<IJwtService>(new JwtService(secretKey));

builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RefreshBeforeValidation = true,
            SaveSigninToken = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        x.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var claimsPrincipal = context.Principal;
                var id = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(id))
                {
                    context.Fail("User ID could not be validated.");
                    return;
                }

                var serviceProvider = context.HttpContext.RequestServices;
                var contaCorrenteRepository = serviceProvider.GetRequiredService<IContaCorrenteRepository>();
                var conta = await contaCorrenteRepository.GetAsync(Guid.Parse(id));

                if (conta is not null && !conta.IsActive)
                {
                    throw new SecurityTokenException();
                }
            }
        };
    });

#endregion

var app = builder.Build();

#region Handle Exception

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is SecurityTokenException validationEx)
        {
            var problem = new
            {
                Status = 401,
                Title = "Não autorizado",
                Detail = "Token inválido"
            };

            context.Response.StatusCode = ((dynamic)problem).Status;
            await context.Response.WriteAsJsonAsync(problem);
        }
    });
});

#endregion

#region Database Migrations

using var provider = app.Services.CreateScope();
var context = provider.ServiceProvider.GetRequiredService<BankMoreContext>();
context.Database.Migrate();

#endregion

app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger().RequireAuthorization();

//app.UseHttpsRedirection();

#region Authorization Configs

app.UseAuthentication();
app.UseAuthorization();

#endregion

app.UseMiddleware<ContextMiddleware>();

app.MapControllers();

app.Run();
