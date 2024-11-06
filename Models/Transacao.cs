namespace QBankApi.Models
{
    public class Transacao
    {
        public int TransacaoID { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string? Tipo { get; set; }

        // Chave estrangeira para Conta de origem e destino
        public int ContaOrigemID { get; set; }
        public Conta? ContaOrigem { get; set; }

        public int? ContaDestinoID { get; set; }
        public Conta? ContaDestino { get; set; }
    }
}