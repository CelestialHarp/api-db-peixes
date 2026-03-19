using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//código refatorado
namespace API_DB_PESCES_em_C__bonitona.Controllers
{
    [ApiController]
    [Route("api/catalogo")]
    public class CatalogoController : ControllerBase
    {
        private readonly CatalogoService _service;

        public CatalogoController(CatalogoService service)
        {
            _service = service;
        }

        // ==========================================
        // ÁREA DE COMPORTAMENTOS
        // ==========================================


        [HttpGet("comportamentos")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> ListarComportamentos()
        {
            var lista = await _service.ListarComportamentos();
            return Ok(lista);
        }

        [HttpPost("comportamentos")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> CriarComportamento([FromBody] CreateComportamentoDTO dto)
        {
            try 
            {
                var novoComportamento = await _service.CriarComportamentoAsync(dto);
                // Como não tenho um método "ObterComportamentoPorId", aponto para a lista mesmo
                // O 'nameof' agora vai funcionar porque o método 'ListarComportamentos' existe logo acima
                return CreatedAtAction(nameof(ListarComportamentos), new { id = novoComportamento.Id }, novoComportamento);
            }
            catch (Exception e)
            {
                //Aplicar isso em outras coisas, achei legalzinho.
                var mensagemReal = e.InnerException?.Message ?? e.Message;
                return BadRequest(new { erro = mensagemReal });
            }
        }

        // ==========================================
        // ÁREA DE ESPÉCIES
        // ==========================================


        [HttpGet("especies")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> ListarEspecies()
        {
            var lista = await _service.ListarEspeciesAsync();
            return Ok(lista);
        }

        [HttpPost("especies")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> CriarEspecie([FromBody] CreateEspecieDTO dto)
        {
            try
            {
                var novaEspecie = await _service.CriarEspecie(dto);
                return CreatedAtAction(nameof(ListarEspecies), new { id = novaEspecie.Id }, novaEspecie);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = e.Message});
            }
        }

        // ==========================================
        // ÁREA DE VITRINE / PESQUISA DE PEIXES
        // ==========================================


        [HttpGet("pesquisar")]
        //Sem [authorize]. Qualquer pessoa na internet deve poder pesquisar sem precisar fazer login.
        public async Task<IActionResult> PesquisarPeixes([FromQuery] string? termoDeBusca)
        {
            try
            {
                // Se o termo vier nulo na URL, passo uma string vazia para a service buscar todos os peixes.
                //Isso vai me permitir mostrar todos os peixes disponíveis de vez, chamando essa rota automaticamente quando o html for carrregado.
                var resultados = await _service.PesquisarPeixesAsync(termoDeBusca ?? string.Empty);
                return Ok(resultados);
            }
            catch (Exception e)
            {
                var mensagemReal = e.InnerException?.Message ?? e.Message;
                return BadRequest(new { erro = mensagemReal });
            }
        }
    }
}