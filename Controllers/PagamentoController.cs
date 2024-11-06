using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly BancoContext _context;

        public PagamentoController(BancoContext context)
        {
            _context = context;
        }

        // GET: api/Pagamento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagamentoDTO>>> GetPagamentos()
        {
            var pagamentos = await _context.Pagamentos
                .Select(p => new PagamentoDTO
                {
                    PagamentoID = p.PagamentoID,
                    Data = p.Data,
                    Valor = p.Valor,
                    ServicoID = p.ServicoID,
                    ContaID = p.ContaID
                })
                .ToListAsync();

            return Ok(pagamentos);
        }

        // GET: api/Pagamento/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PagamentoDTO>> GetPagamento(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
                return NotFound();

            var pagamentoDTO = new PagamentoDTO
            {
                PagamentoID = pagamento.PagamentoID,
                Data = pagamento.Data,
                Valor = pagamento.Valor,
                ServicoID = pagamento.ServicoID,
                ContaID = pagamento.ContaID
            };

            return Ok(pagamentoDTO);
        }

        // POST: api/Pagamento
        [HttpPost]
        public async Task<ActionResult<PagamentoDTO>> CreatePagamento(PagamentoDTO pagamentoDTO)
        {
            var pagamento = new Pagamento
            {
                Data = pagamentoDTO.Data,
                Valor = pagamentoDTO.Valor,
                ServicoID = pagamentoDTO.ServicoID,
                ContaID = pagamentoDTO.ContaID
            };

            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();

            pagamentoDTO.PagamentoID = pagamento.PagamentoID;

            return CreatedAtAction(nameof(GetPagamento), new { id = pagamento.PagamentoID }, pagamentoDTO);
        }

        // PUT: api/Pagamento/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePagamento(int id, PagamentoDTO pagamentoDTO)
        {
            if (id != pagamentoDTO.PagamentoID)
                return BadRequest();

            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
                return NotFound();

            pagamento.Data = pagamentoDTO.Data;
            pagamento.Valor = pagamentoDTO.Valor;
            pagamento.ServicoID = pagamentoDTO.ServicoID;
            pagamento.ContaID = pagamentoDTO.ContaID;

            _context.Entry(pagamento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Pagamento/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePagamento(int id)
        {
            var pagamento = await _context.Pagamentos.FindAsync(id);
            if (pagamento == null)
                return NotFound();

            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
