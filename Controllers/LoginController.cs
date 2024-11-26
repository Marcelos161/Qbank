using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QBankApi.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using QBankApi.Models;
using QBankApi.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly BancoContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(BancoContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO LoginDTO)
    {
        // Busca o usuário pelo CPF
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(u => u.CPF == LoginDTO.CPF);

        if (cliente == null || !BCrypt.Net.BCrypt.Verify(LoginDTO.Senha, cliente.SenhaHash))
        {
            return Unauthorized("Credenciais inválidas.");
        }

        // Gera o token JWT
        var token = GenerateJwtToken(cliente);

        // Retorna os dados do usuário com o token
        var response = new LoginResponseDTO
        {
            ClienteID = cliente.ClienteID,
            Nome = cliente.Nome,
            Token = token
        };

        return Ok(response);
    }

    private string GenerateJwtToken(Cliente cliente)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, cliente.CPF),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("ClienteID", cliente.ClienteID.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
