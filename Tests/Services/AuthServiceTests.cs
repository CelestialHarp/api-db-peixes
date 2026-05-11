using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Models;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;

namespace Tests.Services
{
    public class AuthServiceTests
    {
        private readonly DbPescesContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<DbPescesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            //Takes the Options field of this class that is now configured to run in memory and shoves the field into the variable.
            .Options;
            
            _context = new DbPescesContext(options);
        

            //SUT
            _authService = new AuthService(_context, ReturnMockConfigObj().Object);

        }

        [Fact]
        public async Task Login_SenhaErrada_LancaExcecao()
        {
            //Arrange
            await AdicionarUmUsuárioAoBanco();
            //Act & Assert
            var exception = await Should.ThrowAsync<Exception>( 
                async () => { 
                    await _authService.Login(
                        new LoginDTO (
                            "Username",
                            "senha_incorreta"
                        )
                    );
                }
            );

        }

        

        //Helper methods:
        //Mocking:
        public Mock<IConfiguration> ReturnMockConfigObj()
        {
            var MockedConfig = new Mock<IConfiguration>();
            return MockedConfig;
        }
        //Configuring database:
        public async Task AdicionarUmUsuárioAoBanco()
        {

            _context.Usuarios.Add(new Usuario
            {
               Id = 1,
               Username = "Username",
               PasswordHash =  BCrypt.Net.BCrypt.HashPassword("senha_correta"),
               Role = "Dono"
            });

            await _context.SaveChangesAsync();
        }

        

        

    }
}