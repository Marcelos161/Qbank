using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QBankApi.Data;
using QBankApi.Models;
using QBankApi.DTOs;

namespace QBankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartaoCreditoController : ControllerBase
    {
        private readonly BancoContext _context;

        public CartaoCreditoController(BancoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartaoCreditoDTO>>> GetCartoesCredito()
        {
            var cartoes = await _context.CartoesCredito
                .Select(c => new CartaoCreditoDTO
                {
                    CartaoID = c.CartaoID,
                    NumeroCartao = c.NumeroCartao,
                    Limite = c.Limite,
                    DataValidade = c.DataValidade,
                    ClienteID = c.ClienteID
                })
                .ToListAsync();

            return Ok(cartoes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CartaoCreditoDTO>> GetCartaoCredito(int id)
        {
            var cartao = await _context.CartoesCredito.FindAsync(id);
            if (cartao == null)
                return NotFound();

            var cartaoDTO = new CartaoCreditoDTO
            {
                CartaoID = cartao.CartaoID,
                NumeroCartao = cartao.NumeroCartao,
                Limite = cartao.Limite,
                DataValidade = cartao.DataValidade,
                ClienteID = cartao.ClienteID
            };

            return Ok(cartaoDTO);
        }

        [HttpPost]
        public async Task<ActionResult<CartaoCreditoDTO>> CreateCartaoCredito(CartaoCreditoDTO cartaoDTO)
        {
            var cartao = new CartaoCredito
            {
                NumeroCartao = cartaoDTO.NumeroCartao,
                Limite = cartaoDTO.Limite,
                DataValidade = cartaoDTO.DataValidade,
                ClienteID = cartaoDTO.ClienteID
            };

            _context.CartoesCredito.Add(cartao);
            await _context.SaveChangesAsync();

            cartaoDTO.CartaoID = cartao.CartaoID;

            return CreatedAtAction(nameof(GetCartaoCredito), new { id = cartao.CartaoID }, cartaoDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartaoCredito(int id, CartaoCreditoDTO cartaoDTO)
        {
            if (id != cartaoDTO.CartaoID)
                return BadRequest();

            var cartao = await _context.CartoesCredito.FindAsync(id);
            if (cartao == null)
                return NotFound();

            cartao.NumeroCartao = cartaoDTO.NumeroCartao;
            cartao.Limite = cartaoDTO.Limite;
            cartao.DataValidade = cartaoDTO.DataValidade;
            cartao.ClienteID = cartaoDTO.ClienteID;

            _context.Entry(cartao).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartaoCredito(int id)
        {
            var cartao = await _context.CartoesCredito.FindAsync(id);
            if (cartao == null)
                return NotFound();

            _context.CartoesCredito.Remove(cartao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
