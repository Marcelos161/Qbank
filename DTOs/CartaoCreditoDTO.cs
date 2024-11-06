namespace QBankApi.DTOs
{    
    public class CartaoCreditoDTO
    {
        public int CartaoID { get; set; }
        public string? NumeroCartao { get; set; }
        public decimal Limite { get; set; }
        public DateTime DataValidade { get; set; }

        // Chave estrangeira para Cliente
        public int ClienteID { get; set; }
    }
}