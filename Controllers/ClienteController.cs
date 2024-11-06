using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly BancoContext _context;

        public ClienteController(BancoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .Select(c => new ClienteDTO
                {
                    ClienteID = c.ClienteID,
                    Nome = c.Nome,
                    CPF = c.CPF,
                    DataNascimento = c.DataNascimento,
                    Endereco = c.Endereco
                })
                .ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            var clienteDTO = new ClienteDTO
            {
                ClienteID = cliente.ClienteID,
                Nome = cliente.Nome,
                CPF = cliente.CPF,
                DataNascimento = cliente.DataNascimento,
                Endereco = cliente.Endereco
            };

            return Ok(clienteDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> CreateCliente(ClienteDTO clienteDTO)
        {
            var cliente = new Cliente
            {
                Nome = clienteDTO.Nome,
                CPF = clienteDTO.CPF,
                DataNascimento = clienteDTO.DataNascimento,
                Endereco = clienteDTO.Endereco
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            clienteDTO.ClienteID = cliente.ClienteID;

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteID }, clienteDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, ClienteDTO clienteDTO)
        {
            if (id != clienteDTO.ClienteID)
                return BadRequest();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            cliente.Nome = clienteDTO.Nome;
            cliente.CPF = clienteDTO.CPF;
            cliente.DataNascimento = clienteDTO.DataNascimento;
            cliente.Endereco = clienteDTO.Endereco;

            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
