namespace QBankApi.DTOs;
public class LoginResponseDTO
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Token { get; set; }
}