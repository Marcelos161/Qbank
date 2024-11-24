using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly BancoContext _context;

        public ContaController(BancoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContaDTO>>> GetContas()
        {
            var contas = await _context.Contas
                .Include(c => c.TransacoesOrigem) // Carrega as transações de origem
                .Include(c => c.TransacoesDestino) // Carrega as transações de destino
                .AsNoTracking() // Opcional para melhorar desempenho em leitura
                .ToListAsync();

            var resultado = contas.Select(c => new ContaDTO
            {
                ContaID = c.ContaID,
                NumeroConta = c.NumeroConta,
                Saldo = c.Saldo,
                Tipo = c.Tipo,
                ClienteID = c.ClienteID,
                TransacoesOrigem = c.TransacoesOrigem?.Select(t => new TransacaoDTO
                {
                    TransacaoID = t.TransacaoID,
                    Data = t.Data,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    ContaOrigemID = t.ContaOrigemID,
                    ContaDestinoID = t.ContaDestinoID
                }).ToList(),
                TransacoesDestino = c.TransacoesDestino?.Select(t => new TransacaoDTO
                {
                    TransacaoID = t.TransacaoID,
                    Data = t.Data,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    ContaOrigemID = t.ContaOrigemID,
                    ContaDestinoID = t.ContaDestinoID
                }).ToList()
            });

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContaDTO>> GetConta(int id)
        {
        var conta = await _context.Contas
            .Include(c => c.TransacoesOrigem)   // Carrega as transações de origem
            .Include(c => c.TransacoesDestino)  // Carrega as transações de destino
            .AsNoTracking()                     // Melhor para leitura, pois evita rastreamento de mudanças
            .FirstOrDefaultAsync(c => c.ContaID == id); // Busca a conta com o ID especificado

        if (conta == null)
            return NotFound();

        var contaDTO = new ContaDTO
        {
            ContaID = conta.ContaID,
            NumeroConta = conta.NumeroConta,
            Saldo = conta.Saldo,
            Tipo = conta.Tipo,
            ClienteID = conta.ClienteID,
            // Transações de origem
            TransacoesOrigem = conta.TransacoesOrigem?.Select(t => new TransacaoDTO
            {
                TransacaoID = t.TransacaoID,
                Data = t.Data,
                Valor = t.Valor,
                Tipo = t.Tipo,
                ContaOrigemID = t.ContaOrigemID,
                ContaDestinoID = t.ContaDestinoID
            }).ToList(),
            // Transações de destino
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
