namespace QBankApi.Models
{
    public class Conta
    {
        public int ContaID { get; set; }
        public string? NumeroConta { get; set; }
        public decimal Saldo { get; set; }
        public string? Tipo { get; set; }

        // Chave estrangeira para Cliente
        public int ClienteID { get; set; }
        public Cliente? Cliente { get; set; }

        // Relacionamento: Uma conta pode estar envolvida em várias transações e pagamentos
        public ICollection<Transacao>? TransacoesOrigem { get; set; }
        public ICollection<Transacao>? TransacoesDestino { get; set; }
        public ICollection<Pagamento>? Pagamentos { get; set; }
    }
}