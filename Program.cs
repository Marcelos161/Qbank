using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QBankApi.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicionando serviços ao container
builder.Services.AddControllers();

// Configuração do Banco de Dados
builder.Services.AddDbContext<BancoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
