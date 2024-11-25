using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace QBankApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly BancoContext _context;
        private readonly ILogger<ContaController> _logger;

        public ContaController(BancoContext context, ILogger<ContaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ContaDTO>> GetConta()
        {
            _logger.LogInformation("Iniciando a execução de GetConta Usando token");
            // Extrai o ClienteID das claims do token JWT
            var clienteIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClienteID");
            if (clienteIdClaim == null)
            {
                _logger.LogWarning("Token inválido ou ClienteID ausente.");
                return Unauthorized("Token inválido ou ClienteID ausente.");
            }

            // Converte o ClienteID para um número inteiro
            int clienteId;
            if (!int.TryParse(clienteIdClaim.Value, out clienteId))
            {
                _logger.LogWarning("Cliente ID encontrado no token invalido");
                return Unauthorized("ClienteID inválido no token.");
            }

            // Busca a conta vinculada ao ClienteID
            var conta = await _context.Contas
                .Include(c => c.TransacoesOrigem) // Inclui as transações de origem
                .Include(c => c.TransacoesDestino) // Inclui as transações de destino
                .AsNoTracking() // Melhora o desempenho, desabilitando o rastreamento
                .FirstOrDefaultAsync(c => c.ClienteID == clienteId);

            if (conta == null)
            {
                _logger.LogWarning("conta nao encontrada");
                return NotFound("Conta não encontrada para este cliente.");
            }

            // Mapeia a conta para o DTO
            var contaDTO = new ContaDTO
            {
                ContaID = conta.ContaID,
                NumeroConta = conta.NumeroConta,
                Saldo = conta.Saldo,
                Tipo = conta.Tipo,
                ClienteID = conta.ClienteID,
                TransacoesOrigem = conta.TransacoesOrigem?.Select(t => new TransacaoDTO
                {
                    TransacaoID = t.TransacaoID,
                    Data = t.Data,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    ContaOrigemID = t.ContaOrigemID,
                    ContaDestinoID = t.ContaDestinoID
                }).ToList(),
                TransacoesDestino = conta.TransacoesDestino?.Select(t => new TransacaoDTO
                {
                    TransacaoID = t.TransacaoID,
                    Data = t.Data,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    ContaOrigemID = t.ContaOrigemID,
                    ContaDestinoID = t.ContaDestinoID
                }).ToList()
            };

            return Ok(contaDTO);
        }


        [HttpPost]
        public async Task<ActionResult<ContaDTO>> CreateConta(ContaDTO contaDTO)
        {
            var conta = new Conta
            {
                NumeroConta = contaDTO.NumeroConta,
                Saldo = contaDTO.Saldo,
                Tipo = contaDTO.Tipo,
                ClienteID = contaDTO.ClienteID
            };

            _context.Contas.Add(conta);
            await _context.SaveChangesAsync();

            contaDTO.ContaID = conta.ContaID;

            return CreatedAtAction(nameof(GetConta), new { id = conta.ContaID }, contaDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConta(int id, ContaDTO contaDTO)
        {
            if (id != contaDTO.ContaID)
                return BadRequest();

            var conta = await _context.Contas.FindAsync(id);
            if (conta == null)
                return NotFound();

            conta.NumeroConta = contaDTO.NumeroConta;
            conta.Saldo = contaDTO.Saldo;
            conta.Tipo = contaDTO.Tipo;
            conta.ClienteID = contaDTO.ClienteID;

            _context.Entry(conta).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConta(int id)
        {
            var conta = await _context.Contas.FindAsync(id);
            if (conta == null)
                return NotFound();

            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
