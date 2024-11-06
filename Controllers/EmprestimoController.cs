using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmprestimoController : ControllerBase
    {
        private readonly BancoContext _context;

        public EmprestimoController(BancoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmprestimoDTO>>> GetEmprestimos()
        {
            var emprestimos = await _context.Emprestimos
                .Select(e => new EmprestimoDTO
                {
                    EmprestimoID = e.EmprestimoID,
                    Valor = e.Valor,
                    DataSolicitacao = e.DataSolicitacao,
                    Status = e.Status,
                    ClienteID = e.ClienteID
                })
                .ToListAsync();

            return Ok(emprestimos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmprestimoDTO>> GetEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
                return NotFound();

            var emprestimoDTO = new EmprestimoDTO
            {
                EmprestimoID = emprestimo.EmprestimoID,
                Valor = emprestimo.Valor,
                DataSolicitacao = emprestimo.DataSolicitacao,
                Status = emprestimo.Status,
                ClienteID = emprestimo.ClienteID
            };

            return Ok(emprestimoDTO);
        }

        [HttpPost]
        public async Task<ActionResult<EmprestimoDTO>> CreateEmprestimo(EmprestimoDTO emprestimoDTO)
        {
            var emprestimo = new Emprestimo
            {
                Valor = emprestimoDTO.Valor,
                DataSolicitacao = emprestimoDTO.DataSolicitacao,
                Status = emprestimoDTO.Status,
                ClienteID = emprestimoDTO.ClienteID
            };

            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();

            emprestimoDTO.EmprestimoID = emprestimo.EmprestimoID;

            return CreatedAtAction(nameof(GetEmprestimo), new { id = emprestimo.EmprestimoID }, emprestimoDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmprestimo(int id, EmprestimoDTO emprestimoDTO)
        {
            if (id != emprestimoDTO.EmprestimoID)
                return BadRequest();

            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
                return NotFound();

            emprestimo.Valor = emprestimoDTO.Valor;
            emprestimo.DataSolicitacao = emprestimoDTO.DataSolicitacao;
            emprestimo.Status = emprestimoDTO.Status;
            emprestimo.ClienteID = emprestimoDTO.ClienteID;

            _context.Entry(emprestimo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
                return NotFound();

            _context.Emprestimos.Remove(emprestimo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
