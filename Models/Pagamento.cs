namespace QBankApi.Models
{
    public class Pagamento
    {
        public int PagamentoID { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }

        // Chave estrangeira para Serviço
        public int ServicoID { get; set; }
        public Servico? Servico { get; set; }

        // Chave estrangeira para Conta
        public int ContaID { get; set; }
        public Conta? Conta { get; set; }
    }
}