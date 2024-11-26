namespace QBankApi.DTOs;
public class LoginResponseDTO
{
    public int ClienteID { get; set; }
    public required string Nome { get; set; }
    public required string Token { get; set; }
}