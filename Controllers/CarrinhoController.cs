
using System.Security.Claims;
using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API_DB_PESCES_em_C__bonitona.Controllers
{
    [ApiController]
    [Route("api/carrinho")]
    public class CarrinhoController : ControllerBase
    {
        private readonly CarrinhoService _service;

        //A IDE me sugere usar um construtor primário ao invés desse, depois vou verificar se é razoável ou não.
        public CarrinhoController(CarrinhoService service)
        {
            _service = service;
        }

        private int ObterUsuarioIdDoToken()
        {
            // O .NET desencripta o JWT e coloca as informações no objeto "User"
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Sob condições normais: non-reachable. Se tudo ocorrer certinho, o token sempre vai ter o ID.
            //Isso aqui serve pra quando condições normais não ocorrerem, isto é, algum erro na infraestrutura (físico, lógico não-humano, etc)
            if (string.IsNullOrEmpty(idString))
                throw new UnauthorizedAccessException("O Token não contém o ID do usuário.");

            return int.Parse(idString);
        }

        [HttpGet("obter-carrinho")]        
        [Authorize]
        public async Task<IActionResult> ObterCarrinho()
        {
            try 
            {
            var carrinho = await _service.ObterCarrinhoAsync(ObterUsuarioIdDoToken()); 
            //O try and catch aqui não serve por razões de defesa cibernética: não há possibilidade lógica de manipulação dos valores por vias externas, sob condições normais. Contudo, há ainda uma coisa contra a qual o try-catching seria para mim útil: defesa contra a incompetência alheia e a possibilidade de erro de software (se o banco cair, etc), o que evita a exposição de stack trace de erro na UI, contendo código, e provê uma mensagem de erro educada.
            return Ok(carrinho);
            } catch (Exception ex) //"Ex" é, infelizmente (kkkk), convenção em C#, e não "e". 
            {
                return BadRequest(new { erro = ex.Message});
                /*Pega-tudo pra caso alguma coisa quebre e ocorra uma requisição sem a credencial*/
            }
            
        }

        [HttpPost("adicionar-item")]
        [Authorize]
        public async Task<IActionResult> AdicionarItemAsync(AdicionarItemCarrinhoDTO dto)
        {
            try
            {
                var carrinhoAtualizado = await _service.AdicionarItemAsync(ObterUsuarioIdDoToken(), dto.PesceId);
                return Ok(carrinhoAtualizado);
            } catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message});
            }
        }
        //Não há problema de confiar na informação da UI, uma vez que é uma operação de remoção de um item do próprio carrinho.
        [HttpDelete("itens/{pesceId}")]
        [Authorize]
        public async Task<IActionResult> RemoverItem(int pesceId)
        {

            var carrinhoAtualizado = await _service.RemoverItemAsync(ObterUsuarioIdDoToken(), pesceId);
            return Ok(carrinhoAtualizado);
        }


    }
}