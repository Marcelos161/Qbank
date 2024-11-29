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
            // Recupera o ClienteID do token JWT
            var clienteIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClienteID");
            if (clienteIdClaim == null)
            {
                return Unauthorized("Token inválido ou ClienteID ausente.");
            }

            if (!int.TryParse(clienteIdClaim.Value, out int clienteId))
            {
                return Unauthorized("ClienteID inválido no token.");
            }

            // Recupera a conta de origem do banco de dados
            var contaOrigem = await _context.Contas
                .FirstOrDefaultAsync(c => c.ContaID == transacaoDTO.ContaOrigemID);

            if (contaOrigem == null)
            {
                return NotFound("Conta de origem não encontrada.");
            }

            // Verifica se a conta de origem pertence ao cliente do token
            if (contaOrigem.ClienteID != clienteId)
            {
                return Unauthorized("A conta de origem não pertence ao cliente autenticado.");
            }

            // Verifica se o saldo da conta de origem é suficiente
            if (contaOrigem.Saldo < transacaoDTO.Valor)
            {
                return BadRequest("Saldo insuficiente na conta de origem.");
            }

            // Caso a conta de origem tenha saldo suficiente, cria a transação
            var transacao = new Transacao
            {
                Data = transacaoDTO.Data,
                Valor = transacaoDTO.Valor,
                Tipo = transacaoDTO.Tipo,
                ContaOrigemID = transacaoDTO.ContaOrigemID,
                ContaDestinoID = transacaoDTO.ContaDestinoID
            };

            // Atualiza o saldo da conta de origem
            contaOrigem.Saldo -= transacaoDTO.Valor;

            // Se a transação for de crédito, atualiza o saldo da conta destino também
            if (transacao.Tipo == "Credito")
            {
                var contaDestino = await _context.Contas
                    .FirstOrDefaultAsync(c => c.ContaID == transacaoDTO.ContaDestinoID);

                if (contaDestino == null)
                {
                    return NotFound("Conta de destino não encontrada.");
                }

                contaDestino.Saldo += transacaoDTO.Valor;
            }

            // Adiciona a transação ao contexto e salva as alterações
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            // Define o ID da transação no DTO
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
