namespace QBankApi.DTOs;
public class LoginDTO
{
    public required string CPF { get; set; }
    public required string Senha { get; set; }
}