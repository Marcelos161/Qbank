namespace QBankApi.DTOs
{
    public class ClienteDTO
    {
        public int ClienteID { get; set; }
        public required string Nome { get; set; }
        public required string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Endereco { get; set; }
        public string? Senha { get; set; }

        public ICollection<ContaDTO>? Contas { get; set; }
        public ICollection<EmprestimoDTO>? Emprestimos { get; set; }
        public ICollection<CartaoCreditoDTO>? CartoesCredito { get; set; }
    }
}
