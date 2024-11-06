namespace QBankApi.DTOs
{    
    public class PagamentoDTO
    {
        public int PagamentoID { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }

        // Chave estrangeira para Serviço e Conta
        public int ServicoID { get; set; }
        public int ContaID { get; set; }
    }
}