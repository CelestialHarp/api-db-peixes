//Importações "subentendíveis" para o compilador. Omiti em todos os outros arquivos, mas mantive nesse só pra mostrar.
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_DB_PESCES_em_C__bonitona.Controllers
{
    [ApiController]
    [Route("api/precos")]
    public class PrecosController : ControllerBase
    {
        // engraçado, descobri que essas conexões do service com a controler, que eu fazia já há um tempo (já que eu, antes de mudar de foco, fiz algumas coisas com o Spring Boot), são chamadas de injeção de dependência.
        private readonly PrecoService _service;

        public PrecosController(PrecoService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner")]
        public IActionResult DefinirPreco([FromBody] CreatePrecoDTO dto)
        {
            //Nota para mim: Pesquisar o que exatamente é um "middleware", que descobri que deveria ser utilizado aqui.
            try 
            {
                _service.DefinirPreco(dto);
                return Ok(new { mensagem = "Preço definido com sucesso!" });
            }
            catch (Exception ex)
            {
                // Retorna 400 Bad Request com a mensagem do erro
                return BadRequest(new { erro = ex.Message });
            }
        }
        
        [HttpGet]
        //Implementei esse método porque talvez fosse útil ao trabalhador pesquisar as combinações de preços, talvez pra obter um pouco de insight quando tentando definir o preço de uma nova combinação.
        //Lista o preço para todas as combinações
        [Authorize(Roles = "Admin,Owner")]
        public IActionResult ListarTodos()
        {
            var lista = _service.ListarTodos();
            return Ok(lista);
        }

        //Lista o preço para a combinação específica sendo manipulada pelo usuário
        [HttpGet("sugestao")]
        [Authorize(Roles = "Admin,Owner")]
        public IActionResult ObterSugestao
                            (
                                [FromQuery] int especieId, 
                                [FromQuery] int saudeId, 
                                [FromQuery] int desenvId
                            )
        {
            // Nota: Criar método "ObterUm" no Service, 
            // var preco = _service.ObterPrecoEspecifico(especieId, saudeId, desenvId);
            // return Ok(preco);
            
            return Ok(new { valor = 0 }); // Placeholder
        }

    }
}