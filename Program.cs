using QBankApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/hello", (string name) =>
{
    var saudacao = DateTime.Now.Hour < 12 ? "Bom dia" : "Boa tarde";
    var retorno = $"{saudacao}, {name}";
    return retorno;
});

app.MapGet("/accounts", () =>
{
    // Código para obter todas as contas
});

app.MapGet("/accounts/{id}", (int id) =>
{
    // Código para obter uma conta por ID
});

app.MapPost("/accounts", (Account account) =>
{
    // Código para criar uma nova conta
});

app.MapPut("/accounts/{id}", (int id, Account updatedAccount) =>
{
    // Código para atualizar uma conta existente
});

app.MapDelete("/accounts/{id}", (int id) =>
{
    // Código para deletar uma conta
});

app.Run();
