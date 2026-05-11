using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_DB_PESCES_em_C__bonitona.Models;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Shouldly;

namespace Tests.Services
{
    public class CarrinhoServiceTests
    {
        private readonly DbPescesContext _context;
        private readonly CarrinhoService _carrinhoService;
        public CarrinhoServiceTests()
        {
            var options = new DbContextOptionsBuilder<DbPescesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            //Takes the Options field of this class that is now configured to run in memory and shoves the field into the variable.
            .Options;
            /*
                Gets the options being configured.

                API DB PESCES em C# bonitona - Not Available ⚠
                Tests - Available

                You can use the navigation bar to switch contexts.
                'Options' is not null here.
            */

            _context = new DbPescesContext(options);

            //SUT
            _carrinhoService = new CarrinhoService(_context);

        }
        [Fact]
        public async Task AdicionarItemAsync_PeixeVendido_LancaExcecao()
        {
            //Arrange

            await PopularBanco();            

            //Act & Assert
            var exception = await Should.ThrowAsync<Exception>( async () => 
            {
                await _carrinhoService.AdicionarItemAsync(1,1/*O que tentei antes: _context.Peixes.Find(p => p.id == 1*, isto com o método acessório sendo chamado antes.)*/);
            });

        }



        //Helper Methods:

        private async Task PopularBanco()
        {
            _context.Peixes.Add( new Peixe
            {
                Id = 1,
                EspecieId = 1,
                LoteId = null,
                HealthStateId = 1,
                EstadoDesenvolvimentoId = 1,
                DataNascimento = DateOnly.FromDateTime(DateTime.Now),
                Sexo = "Macho"
            }
            );

            _context.Usuarios.Add( new Usuario
            {
                Id = 1,
                Username = "User",
                PasswordHash = "dfertgghfea",
                Role = "Dono"

            }
            );

            _context.ItensPedido.Add(new ItemPedido
            {
                Id = 1,
                PedidoId = 1,
                PeixeId = 1,
                PrecoNoMomento = 30.00m
            });

            await _context.SaveChangesAsync();
        }
        
    }
}