namespace QBankApi.Models
{
    public class Servico
    {
        public int ServicoID { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        // Relacionamento: Um serviço pode ter vários pagamentos
        public ICollection<Pagamento>? Pagamentos { get; set; }
    }
}
