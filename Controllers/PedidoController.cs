
using System.Security.Claims;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_DB_PESCES_em_C__bonitona.Controllers
{
    [ApiController]
    [Route("api/pedido")]
    public class PedidoController : ControllerBase
    {
        private readonly PedidoService _service;

        /*A IDE sugere construtor primário.*/
        public PedidoController (PedidoService service)
        {
            _service = service;
        }

        private int ObterUsuarioIdDoToken()
        {
            // Código comentado na CarrinhoControler.
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idString))
                throw new UnauthorizedAccessException("O Token não contém o ID do usuário.");

            return int.Parse(idString);
        }

        [HttpPost("finalizar")] //"pedido" está bom?
        [Authorize]
        public async Task<IActionResult> CriarPedido()
        {
            var pedido = await _service.FinalizarCompraAsync(ObterUsuarioIdDoToken());
            return Ok(pedido);
        }

    }
}