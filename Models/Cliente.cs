namespace QBankApi.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public required string Nome { get; set; }
        public required string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Endereco { get; set; }

        // Senha armazenada como hash, nunca em texto simples
        public required string SenhaHash { get; set; }

        // Relacionamento com outras entidades (ex.: Contas, Empréstimos, Cartões de Crédito)
        public ICollection<Conta>? Contas { get; set; }
        public ICollection<Emprestimo>? Emprestimos { get; set; }
        public ICollection<CartaoCredito>? CartoesCredito { get; set; }
    }
}
