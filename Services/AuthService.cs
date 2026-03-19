using API_DB_PESCES_em_C__bonitona.Models;
using API_DB_PESCES_em_C__bonitona.DTOs;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API_DB_PESCES_em_C__bonitona.Services
{
    public class AuthService
    {
        private readonly DbPescesContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(DbPescesContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Login(LoginDTO dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == dto.Username);
            
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                throw new Exception("Usuário ou senha inválidos");
            }

            //Gera o Token JWT (É como uma pulseira de autorização)
            var tokenHandler = new 
            JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "chave_super_secreta_padrao_muito_longa_123");
            
            var tokenDescriptor = new 
            SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, usuario.Username),
                    new(ClaimTypes.Role, usuario.Cargo),
                    new(ClaimTypes.NameIdentifier, usuario.Id.ToString()) // <-- Aqui vai o cargo
                ]),
                Expires = DateTime.UtcNow.AddHours(5), // Token vale por um turno de trabalho
                SigningCredentials = new 
                SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task Registrar(RegistroDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Usuário já existe");

            var novoUsuario = new Usuario
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Cria o Hash seguro
                
                Cargo = "Cliente"
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();
        }
    
    }
}