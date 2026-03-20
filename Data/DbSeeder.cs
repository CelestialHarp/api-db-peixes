
using API_DB_PESCES_em_C__bonitona.Models;

namespace API_DB_PESCES_em_C__bonitona.Data
{
    public static class DbSeeder
    {
        public static void Seed(WebApplication app)
        {
            // Abre um "túnel" de escopo temporário para a base de dados durante o arranque
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DbPescesContext>();
            
            //Começo a popular o banco de dados com alguns registros "mínimos". Apartir da "Área peixes" que começam as coisas completamente desnecessárias, normalmente, mas que são muito úteis pra agilizar debugging.
            // 1. O Usuário Gênesis (Dono)
            if (!context.Usuarios.Any())
            {
                context.Usuarios.Add(new Usuario
                {
                    Username = "username",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                    Cargo = "Owner"
                });
            }

            // 2. Comportamentos Base
            if (!context.Comportamentos.Any())
            {
                context.Comportamentos.AddRange(
                    
                    new Comportamento { Nome = "Gregário intraespecífico", Descricao = "Vive em cardumes da mesma espécie." },
                    new Comportamento { Nome = "Gregário interespecífico", Descricao = "Convive bem em cardumes com outras espécies." },
                    new Comportamento { Nome = "Territorial Intraespecífico", Descricao = "Defende ativamente uma zona do aquário (tocas ou fundo) de peixes da mesma espécie." },
                    new Comportamento { Nome = "Territorial Interespecífico", Descricao = "Defende ativamente uma zona do aquário (tocas ou fundo) de peixes de qualquer espécie." }
                );
            }

            // 3. Estados de Saúde
            if (!context.EstadosSaudes.Any())
            {
                context.EstadosSaudes.AddRange(
                    new EstadoSaude { Descricao = "Saudável" },
                    new EstadoSaude { Descricao = "Doente" },
                    new EstadoSaude { Descricao = "Em Tratamento" }
                );
            }

            // 4. Estados de Desenvolvimento
            if (!context.EstadosDesenvolvimentos.Any())
            {
                context.EstadosDesenvolvimentos.AddRange(
                    new EstadoDesenvolvimento { Descricao = "Alevino" },
                    new EstadoDesenvolvimento { Descricao = "Juvenil" },
                    new EstadoDesenvolvimento { Descricao = "Adulto" }
                );
            }

            // 5. Guildas Tróficas
            if (!context.GuildasTroficas.Any())
            {
                context.GuildasTroficas.AddRange(
                    new GuildaTrofica { Nome = "Piscívoros" },
                    new GuildaTrofica { Nome = "Invertívoro" },
                    new GuildaTrofica { Nome = "Herbívoro" },
                    new GuildaTrofica { Nome = "Onívoro" },
                    new GuildaTrofica { Nome = "Detritívoros" }
                );
            }
            context.SaveChanges();

            /*
               ======================================
               |       ÁREA ESPÉCIES E PEIXES       |
               ======================================
            */
                // 6. Espécies, Peixes (Estoque) e Preços
                if (!context.Especies.Any())
                {
                    // Puxamos as IDs baseadas nas descrições para não chutar números mágicos
                    var comportamentoTerritorialInter = context.Comportamentos.FirstOrDefault(c => c.Nome == "Territorial Interespecífico")?.Id ?? 1;
                    var comportamentoTerritorialIntra = context.Comportamentos.FirstOrDefault(c => c.Nome == "Territorial Intraespecífico")?.Id ?? 1;
                    var comportamentoGregárioInter = context.Comportamentos.FirstOrDefault(c => c.Nome == "Gregário interespecífico")?.Id ?? 1;
                    var ComportamentoGregárioIntra = context.Comportamentos.FirstOrDefault(c => c.Nome == "Gregário intraespecífico")?.Id ?? 1;

                    var guildaOnivoro = context.GuildasTroficas.FirstOrDefault(g => g.Nome == "Onívoro")?.Id ?? 1;
                    var guildaPiscivoro = context.GuildasTroficas.FirstOrDefault(g => g.Nome == "Piscívoros")?.Id ?? 1;

                    var saudeSaudavel = context.EstadosSaudes.FirstOrDefault(e => e.Descricao == "Saudável")?.Id ?? 1;
                    var desenvAdulto = context.EstadosDesenvolvimentos.FirstOrDefault(e => e.Descricao == "Adulto")?.Id ?? 1;

                    //Daqui pra cima, é tudo inútil, daqui pra baixo, normalmente se eliminaria.

                    // A. CRIANDO AS ESPÉCIES
                    var especiesParaAdicionar = new List<Especie>
                    {
                        new Especie { NomeVulgar = "Tilápia", Taxon = "Oreochromis niloticus", ImagemUrl = "tilapia.png", ComportamentoId = comportamentoTerritorialInter, GuildaTroficaId = guildaOnivoro },
                        new Especie { NomeVulgar = "Betta", Taxon = "Betta splendens", Linhagem = "Halfmoon", ImagemUrl = "betta-splendens.png", ComportamentoId = comportamentoTerritorialIntra, GuildaTroficaId = guildaPiscivoro },
                        new Especie { NomeVulgar = "Acará-Bandeira", Taxon = "Pterophyllum scalare", ImagemUrl = "acara-bandeira.png", ComportamentoId = ComportamentoGregárioIntra, GuildaTroficaId = guildaOnivoro },
                        new Especie { NomeVulgar = "Acará-Disco", Taxon = "Symphysodon aequifasciatus", ImagemUrl = "acara-disco.png", ComportamentoId = ComportamentoGregárioIntra, GuildaTroficaId = guildaOnivoro },
                        new Especie { NomeVulgar = "Guppy", Taxon = "Poecilia reticulata", ImagemUrl = "guppy.png", ComportamentoId = comportamentoGregárioInter, GuildaTroficaId = guildaOnivoro },
                        new Especie { NomeVulgar = "Carpa Koi", Taxon = "Cyprinus rubrofuscus", ImagemUrl = "koi.png", ComportamentoId = comportamentoGregárioInter, GuildaTroficaId = guildaOnivoro }
                    };
                    
                    context.Especies.AddRange(especiesParaAdicionar);
                    context.SaveChanges(); // Salva para gerar os IDs das espécies

                    // B. CRIANDO OS PEIXES FÍSICOS (1 de cada para a vitrine) E OS PREÇOS
                    // Tabela de preços de mercado médios
                    var tabelaDePrecos = new Dictionary<string, decimal>
                    {
                        { "Tilápia", 50.00m },
                        { "Betta", 35.00m },
                        { "Acará-Bandeira", 25.00m },
                        { "Acará-Disco", 150.00m }, // Peixe caro
                        { "Guppy", 5.00m },         // Peixe barato
                        { "Carpa Koi", 80.00m }
                    };

                    foreach (var especie in context.Especies.ToList())
                    {
                        // Adiciona o peixe no estoque
                        context.Pesces.Add(new Pesce 
                        { 
                            EspecieId = especie.Id,
                            EstadoSaudeId = saudeSaudavel,
                            EstadoDesenvolvimentoId = desenvAdulto,
                            Sexo = especie.NomeVulgar == "Betta" ? "Macho" : null // Bettas vendidos costumam ser machos por causa da cauda! deixei assim pq n tem problema, nesse caso.
                        });

                        // Tabela de Preços cruzando a Espécie com a Saúde e Desenvolvimento
                        if (tabelaDePrecos.TryGetValue(especie.NomeVulgar, out decimal precoVenda))
                        {
                            context.Precos.Add(new Preco
                            {
                                Valor = precoVenda,
                                EspecieId = especie.Id,
                                EstadoSaudeId = saudeSaudavel,
                                EstadoDesenvolvimentoId = desenvAdulto
                            });
                        }
                    }

                    context.SaveChanges();
                }
            /*
               ======================================
               |   FIM DA ÁREA ESPÉCIES E PEIXES    |
               ======================================
            */

            // Salva tudo de uma vez no PostgreSQL
            context.SaveChanges();
        }
    }
}