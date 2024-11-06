using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicoController : ControllerBase
    {
        private readonly BancoContext _context;

        public ServicoController(BancoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicoDTO>>> GetServicos()
        {
            var servicos = await _context.Servicos
                .Select(s => new ServicoDTO
                {
                    ServicoID = s.ServicoID,
                    Nome = s.Nome,
                    Descricao = s.Descricao
                })
                .ToListAsync();

            return Ok(servicos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServicoDTO>> GetServico(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
                return NotFound();

            var servicoDTO = new ServicoDTO
            {
                ServicoID = servico.ServicoID,
                Nome = servico.Nome,
                Descricao = servico.Descricao
            };

            return Ok(servicoDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ServicoDTO>> CreateServico(ServicoDTO servicoDTO)
        {
            var servico = new Servico
            {
                Nome = servicoDTO.Nome,
                Descricao = servicoDTO.Descricao
            };

            _context.Servicos.Add(servico);
            await _context.SaveChangesAsync();

            servicoDTO.ServicoID = servico.ServicoID;

            return CreatedAtAction(nameof(GetServico), new { id = servico.ServicoID }, servicoDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServico(int id, ServicoDTO servicoDTO)
        {
            if (id != servicoDTO.ServicoID)
                return BadRequest();

            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
                return NotFound();

            servico.Nome = servicoDTO.Nome;
            servico.Descricao = servicoDTO.Descricao;

            _context.Entry(servico).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServico(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
                return NotFound();

            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
