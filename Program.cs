using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QBankApi.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Adicionando serviços ao container
builder.Services.AddControllers();

// Configuração do Banco de Dados
builder.Services.AddDbContext<BancoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Valida o emissor
            ValidateAudience = true, // Valida o público
            ValidateLifetime = true, // Valida o tempo de expiração
            ValidateIssuerSigningKey = true, // Valida a chave de assinatura

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // Emissor válido
            ValidAudience = builder.Configuration["JwtSettings:Audience"], // Público válido
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])) // Chave de assinatura
        };
    });

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5168); // HTTP
        options.ListenLocalhost(7266, listenOptions => // HTTPS
        {
            listenOptions.UseHttps();
        });
    });

builder.Services.AddAuthorization();

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Adiciona logs no console
builder.Logging.AddDebug(); // Adiciona logs no debugger (se disponível)

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    // Habilitando o Swagger em modo de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware para redirecionamento de HTTPS
app.UseHttpsRedirection();

// Middleware para autenticação e autorização
app.UseAuthentication(); // Habilita autenticação
app.UseAuthorization(); // Habilita autorização

// Mapeamento dos controladores
app.MapControllers();

// Iniciando a aplicação
app.Run();
