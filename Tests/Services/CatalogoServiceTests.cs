using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using API_DB_PESCES_em_C__bonitona.Models;


namespace Tests.Services;

public class CatalogoServiceTests
{
    private readonly DbPescesContext _context;
    private readonly CatalogoService _catalogoService;
    public CatalogoServiceTests()
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
        _catalogoService = new CatalogoService(_context);

    }
    
    [Fact]
    public async Task CriarComportamentoAsync_DadosValidos_SalvaNoBancoERetornaUmDTO()
    {
        //Arrange

        //Creating a real DTO because data objects are harmless to the isolation of the unity.

        /*Isso aqui dá:
        There is no argument given that corresponds to the required parameter 'Nome' of 'CreateComportamentoDTO.CreateComportamentoDTO(string, string)', mas óbviamente tem o Nome recebendo string ali. Eu resolvi colocar como "criação antiga de objeto", com () ao invés de {} com as atribuições, e foi normal, sem o erro, o que é ainda mais estranho, porque mostra que parece ser algum erro estranhíssimo com a variável Nome.
        */
        var dto = new CreateComportamentoDTO
        (
            "Gregário intraespecífico",
            "Vive em cardumes da mesma espécie."
        );

        //Criar variáveis, instanciar as classes, e configurar os mocks.

        
        //Act
        //Chamar um método, atribuindo seu resultado à uma variável.
        var result = await _catalogoService.CriarComportamentoAsync(dto);

        var savedBehavior = await _context.Comportamentos.FirstOrDefaultAsync(c => c.Nome == "Gregário intraespecífico");

        //Assert
        

        //Assegurar os resultados

        //Testes de regressão
        result.ShouldNotBeNull();
        result.Nome.ShouldBe("Gregário intraespecífico");
        result.Descricao.ShouldBe("Vive em cardumes da mesma espécie.");
        result.Id.ShouldBeGreaterThan(0);

        savedBehavior.ShouldNotBeNull();
        savedBehavior.Descricao.ShouldBe(dto.Descricao);



    }
    //Fun fact: I discovered that, insterestingly, I do not have to instantiate local RAM databases inside each test method, since the framework literaly instatiate and destroy the ENTIRE CLASS for each method lllllll.
    //Actually it is pretty useful because this way there is less code on the screen and it becomes more readable.

    
    [Fact]
    public async Task PesquisarPeixesAsync_PesquisaComAlgumPeixeEmOutroCarrinho_RetornaSemOPeixe()
    {
        //Arrange

        //Nota de estudo: Literalmente, toda vez que estiver executando testes sobre métodos que façam inclusão (inner joins) via chave estrangeira, como neste método que procura as informações adequadas a um peixe em todas as tabelas ligadas, devo, sob pena da linha ser descartada em runtime por inexistencia de informação resultante da chave estrangeira, incluir, literalmente, todas as tabelas relacionadas (pesquisar se as tabelas pais das tabelas pais das tabelas filhas, e assim por diante, também devem ser instanciadas). O savechanges() não dá erro porque o banco In-Memory não valiada a integridade das chaves estrangeiras. Caso não houvessem esses includes no método, em CatalogoService, não haveria necessidade de instanciar as tabelas quando resultados que não envolvem registros delas são testados. (esse método implementa )
        await PopularBancoComTresPeixes();

        //Act

        var result = await _catalogoService.PesquisarPeixesAsync("Tilápia");
        //Since now one the fish is on a market kart, the remaining fish must be 2

        //Assert
        
        //Regression Tests:
        
        //BUSINESS RULE: Fish that are on market karts should not be searchable in the UI.
        //Conditions (I need a better word to express the sense of what is the actual envirionment in which these things live in, which affects the output of the tests, since 'conditions' could be misinterpreted as a rule or smth):Since now one the fish is on a market kart, the remaining fish must be 2

        result.Count.ShouldBe(2);

    }

    //Helper Methods:

    private async Task PopularBancoComTresPeixes()
    {
        _context.Peixes.AddRange(
        new Peixe
        {
            EspecieId = 1,
            LoteId = null,
            HealthStateId = 1,
            EstadoDesenvolvimentoId = 1,
            DataNascimento = DateOnly.FromDateTime(DateTime.Now),
            Sexo = "Macho"
        },
        new Peixe
        {
            EspecieId = 1,
            LoteId = null,
            HealthStateId = 1,
            EstadoDesenvolvimentoId = 1,
            DataNascimento = DateOnly.FromDateTime(DateTime.Now),
            Sexo = "Macho"
        },
        new Peixe
        {
            EspecieId = 1,
            LoteId = null,
            HealthStateId = 1,
            EstadoDesenvolvimentoId = 1,
            DataNascimento = DateOnly.FromDateTime(DateTime.Now),
            Sexo = "Macho"
        }
        );
        //Porque dá "Unnecessary assignment of a value to 'ItemCarrinho'"?/
        
        _context.ItensCarrinho.Add(new ItemCarrinho
        {
            Id = 1,
            CarrinhoId = 1,
            PeixeId = 1
        });

        _context.Especies.Add(new Especie
        {
            Id = 1,

            NomeVulgar = "Tilápia",

            Taxon  = "Oreochromis Niloticus",

            Subespecie = null,

            Linhagem  = null,

            ImagemUrl = null,

            Comentario = null,

            ComportamentoId = null,

            
            GuildaTroficaId = 1
        });
        
        _context.EstadosSaude.Add(new EstadoSaude
        {
            Id = 1,
            Descricao = "Saudável"
        });
        
        _context.EstadosDesenvolvimento.Add(new EstadoDesenvolvimento
        {
            Id = 1,
            Descricao = "Adulto"
        });

        _context.GuildasTroficas.Add(new GuildaTrofica
        {
            Id = 1,
            Nome = "Onívoro"
        });


        await _context.SaveChangesAsync();
    }

}



