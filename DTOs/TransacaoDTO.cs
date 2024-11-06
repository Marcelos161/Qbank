namespace QBankApi.DTOs
{    
    public class TransacaoDTO
    {
        public int TransacaoID { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string? Tipo { get; set; }

        // Chave estrangeira para as contas envolvidas
        public int ContaOrigemID { get; set; }
        public int? ContaDestinoID { get; set; }
    }
}