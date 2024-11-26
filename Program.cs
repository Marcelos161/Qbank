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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Autenticação falhou: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validado com sucesso.");
                return Task.CompletedTask;
            }
        };
    });
    builder.Services.AddAuthorization();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5168); // HTTP
        options.ListenLocalhost(7266, listenOptions => // HTTPS
        {
            listenOptions.UseHttps();
        });
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Adiciona logs no console
builder.Logging.AddDebug(); // Adiciona logs no debugger (se disponível)

var app = builder.Build();

    // Log HTTP requests e respostas
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Requisição recebida: {Method} {Url}", context.Request.Method, context.Request.Path);
    
    await next.Invoke();
    logger.LogInformation("Resposta enviada: {StatusCode}", context.Response.StatusCode);
});
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
