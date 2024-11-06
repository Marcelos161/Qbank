using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly BancoContext _context;

        public TransacaoController(BancoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> GetTransacoes()
        {
            var transacoes = await _context.Transacoes
                .Select(t => new TransacaoDTO
                {
                    TransacaoID = t.TransacaoID,
                    Data = t.Data,
                    Valor = t.Valor,
                    Tipo = t.Tipo,
                    ContaOrigemID = t.ContaOrigemID,
                    ContaDestinoID = t.ContaDestinoID
                })
                .ToListAsync();

            return Ok(transacoes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransacaoDTO>> GetTransacao(int id)
        {
            var transacao = await _context.Transacoes.FindAsync(id);
            if (transacao == null)
                return NotFound();

            var transacaoDTO = new TransacaoDTO
            {
                TransacaoID = transacao.TransacaoID,
                Data = transacao.Data,
                Valor = transacao.Valor,
                Tipo = transacao.Tipo,
                ContaOrigemID = transacao.ContaOrigemID,
                ContaDestinoID = transacao.ContaDestinoID
            };

            return Ok(transacaoDTO);
        }

        [HttpPost]
        public async Task<ActionResult<TransacaoDTO>> CreateTransacao(TransacaoDTO transacaoDTO)
        {
            var transacao = new Transacao
            {
                Data = transacaoDTO.Data,
                Valor = transacaoDTO.Valor,
                Tipo = transacaoDTO.Tipo,
                ContaOrigemID = transacaoDTO.ContaOrigemID,
                ContaDestinoID = transacaoDTO.ContaDestinoID
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            transacaoDTO.TransacaoID = transacao.TransacaoID;

            return CreatedAtAction(nameof(GetTransacao), new { id = transacao.TransacaoID }, transacaoDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransacao(int id, TransacaoDTO transacaoDTO)
        {
            if (id != transacaoDTO.TransacaoID)
                return BadRequest();

            var transacao = await _context.Transacoes.FindAsync(id);
            if (transacao == null)
                return NotFound();

            transacao.Data = transacaoDTO.Data;
            transacao.Valor = transacaoDTO.Valor;
            transacao.Tipo = transacaoDTO.Tipo;
            transacao.ContaOrigemID = transacaoDTO.ContaOrigemID;
            transacao.ContaDestinoID = transacaoDTO.ContaDestinoID;

            _context.Entry(transacao).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransacao(int id)
        {
            var transacao = await _context.Transacoes.FindAsync(id);
            if (transacao == null)
                return NotFound();

            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
