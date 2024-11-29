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
        public async Task<ActionResult<IEnumerable<ContaDTO>>> GetContas()
        {
            _logger.LogInformation("Iniciando a execução de GetContas usando token");

            // Extrai o ClienteID das claims do token JWT
            var clienteIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClienteID");
            if (clienteIdClaim == null)
            {
                _logger.LogWarning("Token inválido ou ClienteID ausente.");
                return Unauthorized("Token inválido ou ClienteID ausente.");
            }

            // Converte o ClienteID para um número inteiro
            if (!int.TryParse(clienteIdClaim.Value, out int clienteId))
            {
                _logger.LogWarning("Cliente ID encontrado no token é inválido.");
                return Unauthorized("ClienteID inválido no token.");
            }

            // Busca todas as contas vinculadas ao ClienteID
            var contas = await _context.Contas
                .Where(c => c.ClienteID == clienteId)
                .AsNoTracking() // Melhora o desempenho, desabilitando o rastreamento
                .ToListAsync();

            if (contas == null || !contas.Any())
            {
                _logger.LogWarning("Nenhuma conta encontrada para o ClienteID: {ClienteID}", clienteId);
                return NotFound("Nenhuma conta encontrada para este cliente.");
            }

            // Mapeia as contas para uma lista de DTOs
            var contasDTO = contas.Select(conta => new ContaDTO
            {
                ContaID = conta.ContaID,
                NumeroConta = conta.NumeroConta,
                Tipo = conta.Tipo,
            }).ToList();

            return Ok(contasDTO);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ContaDTO>> GetConta(int id)
        {
            _logger.LogInformation("Iniciando a execução de GetConta usando token");

            // Extrai o ClienteID das claims do token JWT
            var clienteIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClienteID");
            if (clienteIdClaim == null)
            {
                _logger.LogWarning("Token inválido ou ClienteID ausente.");
                return Unauthorized("Token inválido ou ClienteID ausente.");
            }

            // Converte o ClienteID para um número inteiro
            if (!int.TryParse(clienteIdClaim.Value, out int clienteId))
            {
                _logger.LogWarning("ClienteID encontrado no token é inválido.");
                return Unauthorized("ClienteID inválido no token.");
            }

            // Busca a conta pelo ID e valida o ClienteID
            var conta = await _context.Contas
                .Include(c => c.TransacoesOrigem) // Inclui as transações de origem
                .Include(c => c.TransacoesDestino) // Inclui as transações de destino
                .AsNoTracking() // Melhora o desempenho, desabilitando o rastreamento
                .FirstOrDefaultAsync(c => c.ContaID == id && c.ClienteID == clienteId);

            if (conta == null)
            {
                _logger.LogWarning("Conta não encontrada ou não pertence ao ClienteID: {ClienteID}", clienteId);
                return NotFound("Conta não encontrada ou não associada a este cliente.");
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
                _logger.LogWarning("Cliente ID encontrado no token inválido: {ClienteId}", clienteIdClaim.Value);
                return Unauthorized("ClienteID inválido no token.");
            }
                _logger.LogWarning("Cliente ID encontrado no token inválido: {ClienteId}", clienteIdClaim.Value);
            var conta = new Conta
            {
                NumeroConta = contaDTO.NumeroConta,
                Saldo = contaDTO.Saldo,
                Tipo = contaDTO.Tipo,
                ClienteID = clienteId
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
