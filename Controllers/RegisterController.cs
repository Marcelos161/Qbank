using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly BancoContext _context;

        public RegisterController(BancoContext context)
        {
            _context = context;
        }

        // Endpoint para registrar um novo cliente
        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> RegistrarCliente([FromBody] ClienteDTO clienteDTO)
        {
            // Validação de dados (exemplo: verificar se o CPF já está registrado)
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.CPF == clienteDTO.CPF);

            if (clienteExistente != null)
            {
                return BadRequest("Já existe um cliente com este CPF.");
            }

            // Gerar o hash da senha
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(clienteDTO.Senha);

            // Criar um novo objeto Cliente
            var novoCliente = new Cliente
            {
                Nome = clienteDTO.Nome,
                CPF = clienteDTO.CPF,
                DataNascimento = clienteDTO.DataNascimento,
                Endereco = clienteDTO.Endereco,
                SenhaHash = senhaHash // Armazenando o hash da senha
            };

            // Adicionar o cliente no banco de dados
            _context.Clientes.Add(novoCliente);
            await _context.SaveChangesAsync();

            // Retornar os dados do novo cliente com o ID gerado
            clienteDTO.ClienteID = novoCliente.ClienteID;

            return CreatedAtAction(nameof(ClienteController.GetCliente), new { id = novoCliente.ClienteID }, clienteDTO);
        }
    }
}
