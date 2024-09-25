var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/hello", (string name) =>
{
    var saudacao = DateTime.Now.Hour < 12 ? "Bom dia" : "Boa tarde";
    var retorno = $"{saudacao}, {name}";
    return retorno;
});

app.MapGet("/pessoa", () =>
{
    var pessoa = new Pessoa
    {
        Nome = "Fulano de Tal",
        DataNascimento = new DateTime(1980, 1, 1),
        Email = "fulano@fulano.com",
        Telefone = "11 99999-9999",
        Endereco = "Rua das Flores",
        Cidade = "São Paulo",
        Estado = "SP",
        Pais = "Brasil",
        CEP = "00000-000",
        CPF = "000.000.000-00",
        RG = "00.000.000-0",
        CNH = "00000000000",
    };
    return pessoa;
});

namespace ApiAula1
{
    public class Pessoa
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string RG { get; set; } = string.Empty;
        public string CNH { get; set; } = string.Empty;
        public string TituloEleitor { get; set; } = string.Empty;
        public string Passaporte { get; set; } = string.Empty;
        public string CarteiraTrabalho { get; set; } = string.Empty;
    }
}
app.Run();
