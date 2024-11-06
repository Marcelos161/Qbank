namespace QBankApi.DTOs
{    
    public class EmprestimoDTO
    {
        public int EmprestimoID { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string? Status { get; set; }

        // Chave estrangeira para Cliente
        public int ClienteID { get; set; }
    }
}