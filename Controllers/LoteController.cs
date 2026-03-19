
using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_DB_PESCES_em_C__bonitona.Controllers
{
    [ApiController]
    [Route("api/lotes")]
    public class LoteController : ControllerBase
    {
        private readonly LoteService _service;

        public LoteController(LoteService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarLote([FromBody] CreateLoteDTO dto)
        {
            var lote = await _service.CriarLoteAsync(dto);
            return CreatedAtAction(nameof(ObterDetalhes), new { id = lote.Id }, lote);
        }

        //GET de lista de peixes de cada lote
        [HttpGet("{id}")] //Talvez eu devesse esconder o id ao mandar a informação pra fora, por segurança.
        public async Task<IActionResult> ObterDetalhes(int id)
        {
            try
            {
                var lote = await _service.ObterDetalhesLoteAsync(id);
                return Ok(lote);
            }
            catch (Exception e)
            {
                return NotFound(new { erro = e.Message});
            }
        }

        [HttpPost]
        [Route("finalizar-venda")]
        public async Task<IActionResult> FinalizarVenda(int id, [FromBody] decimal precoFinal)
        {
            try
            {
                await _service.FinalizarVendaAsync(id, precoFinal);
                return Ok(new { mensagem = "Lote vendido e fechado com sucesso."});
            }
            catch (InvalidOperationException e)
            {
                return Conflict(new { erro = e.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
