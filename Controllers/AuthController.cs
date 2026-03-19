using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_DB_PESCES_em_C__bonitona.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDTO dto)
        {
            try
            {
                // O Service encarrega-se de forçar o Cargo = "Cliente" e de fazer o Hash da senha
                await _authService.Registrar(dto);
                return Ok(new { mensagem = "Registo efetuado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                // O AuthService verifica a senha e devolve a string do JWT
                var token = await _authService.Login(dto);
                
                // O UserResponseDTO embrulha a resposta de forma limpa
                // Como o AuthService atual devolve apenas o token, coloquei "N/A" no cargo por agora.
                // (No futuro, o AuthService poderá devolver o usuário completo para preencher isso corretamente).
                var resposta = new UserResponseDTO(dto.Username, token, "N/A"); //"Not Assigned"
                
                return Ok(resposta);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { erro = ex.Message }); // 401 Unauthorized é o código correto para falha de login
            }
        }
    }
}