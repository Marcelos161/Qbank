namespace QBankApi.DTOs
{
    public class ClienteDTO
    {
        public int ClienteID { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Endereco { get; set; }

        // Inclui informações sobre contas associadas, se necessário
        public ICollection<ContaDTO>? Contas { get; set; }
    }
}