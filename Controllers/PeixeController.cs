using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_DB_PESCES_em_C__bonitona.Controllers
{
    [ApiController]
    [Route("api/peixes")]
    public class PeixeController : ControllerBase
    {
        private readonly PeixeService _service;

        public PeixeController(PeixeService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner")] // Só Admins e o Owner podem adicionar peixes 
        //se fosse sem o (), seriam todos que possuam cadastro (visitantes quaisquer, não).
        public async Task<IActionResult> Adicionar([FromBody] CreatePeixeDTO dto)
        {
            try
            {
                var resultado = await _service.AdicionarPeixe(dto);
                // Retorna 201 Created com o location header (boa prática)
                return CreatedAtAction(nameof(PesquisarPeixe), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }      

        // GET /api/peixes/pesquisar?termo=x&id=y
        [HttpGet("pesquisar")]
        public async Task<IActionResult> PesquisarPeixe(
            [FromQuery] string? peixe,
            [FromQuery] int? ID
        )
        {
            var resultados = await _service.PesquisarPeixeAsync(peixe, ID);
            if (resultados == null || !resultados.Any())
            {
                return Ok(new List<PeixeResponseDTO>()); // Retorna lista vazia []
            }

            return Ok(resultados);
        }


    }
}