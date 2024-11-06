namespace QBankApi.DTOs
{
    public class ContaDTO
    {
        public int ContaID { get; set; }
        public string? NumeroConta { get; set; }
        public decimal Saldo { get; set; }
        public string? Tipo { get; set; }

        // Chave estrangeira para Cliente
        public int ClienteID { get; set; }

        // Inclui informações sobre transações associadas, se necessário
        public ICollection<TransacaoDTO>? TransacoesOrigem { get; set; }
        public ICollection<TransacaoDTO>? TransacoesDestino { get; set; }
    }
}