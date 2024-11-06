namespace QBankApi.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Endereco { get; set; }

        // Relacionamento: Um cliente pode ter várias contas, empréstimos, cartões de crédito
        public ICollection<Conta>? Contas { get; set; }
        public ICollection<Emprestimo>? Emprestimos { get; set; }
        public ICollection<CartaoCredito>? CartoesCredito { get; set; }
    }
}