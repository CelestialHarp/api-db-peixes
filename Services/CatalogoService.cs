
using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Models;
using Microsoft.EntityFrameworkCore;

namespace API_DB_PESCES_em_C__bonitona.Services
{
    public class CatalogoService
    {
        private readonly DbPescesContext _context;

        public CatalogoService(DbPescesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseEspecieDTO>> ListarEspeciesAsync()
        {
            return await _context.Especies
                .AsNoTracking() 
                .Select(e => new ResponseEspecieDTO(e.Id, e.NomeVulgar, e.Taxon, e.Subespecie, e.Linhagem, e.ImagemUrl, e.Comentario, e.Comportamento.Descricao, e.GuildaTrofica.Nome))
                .ToListAsync(); 
        }

        public async Task<IEnumerable<ResponseComportamentoDTO>> ListarComportamentos()
        {
            return await _context.Comportamentos
                .AsNoTracking()
                .Select(c => new ResponseComportamentoDTO(c.Id, c.Nome, c.Descricao))
                .ToListAsync();
        }

        public async Task<ResponseComportamentoDTO> CriarComportamentoAsync(CreateComportamentoDTO dto)
        {
            var novoComportamento = new Comportamento 
            { 
                Nome = dto.Nome, 
                Descricao = dto.Descricao 
            };

            _context.Comportamentos.Add(novoComportamento);
            await _context.SaveChangesAsync();

            return new ResponseComportamentoDTO(novoComportamento.Id, novoComportamento.Nome, novoComportamento.Descricao);
        }

        public async Task<ResponseEspecieDTO> CriarEspecie(CreateEspecieDTO dto)
        {            
            var novaEspecie = new Especie 
            {
                NomeVulgar = dto.NomeVulgar,
                Taxon = dto.Taxon,
                Subespecie = dto.Subespecie,
                Linhagem = dto.Linhagem,
                ImagemUrl = dto.ImagemUrl,
                Comentario = dto.Comentario,
                ComportamentoId = dto.ComportamentoId,
                GuildaTroficaId = dto.GuildaTroficaId
            };

            _context.Especies.Add(novaEspecie);
            await _context.SaveChangesAsync();

            var comportamento = await _context.Comportamentos.FindAsync(dto.ComportamentoId);
            var guilda = await _context.GuildasTroficas.FindAsync(dto.GuildaTroficaId);

            return new ResponseEspecieDTO(novaEspecie.Id, novaEspecie.NomeVulgar, novaEspecie.Taxon, novaEspecie.Subespecie, novaEspecie.Linhagem, novaEspecie.ImagemUrl, novaEspecie.Comentario, comportamento?.Descricao ?? "N/A", guilda?.Nome ?? "N/A");
        }

        // ==========================================
        //            ÁREA DA PESQUISA
        // ==========================================
        public async Task<List<PeixeResponseDTO>> PesquisarPeixesAsync(string termoDeBusca)
        {
            // Subo os joins para o contexto, pra poder realizar as pesquisas.
            var query = _context.Pesces
                .Include(p => p.Especie)
                    .ThenInclude(e => e.Comportamento)
                .Include(p => p.Especie)
                    .ThenInclude(e => e.GuildaTrofica)
                .Include(p => p.Especie)
                    .ThenInclude(e => e.Precos)
                .Include(p => p.EstadoSaude)
                .Include(p => p.EstadoDesenvolvimento)
                .AsQueryable();
            //Retiro os itens do carrinho
            query = query.Where(p => 
                !_context.ItensCarrinho.Any(ic => ic.PesceId == p.Id) && 
                !_context.ItensPedido.Any(ip => ip.PesceId == p.Id)
            );

            // Adiciono o filtro ILike, pra poder pesquisar por termos que contenham os caracteres digitados pelo usuário (em ordem).
            if (!string.IsNullOrWhiteSpace(termoDeBusca))
            {
                query = query.Where(p => 
                    EF.Functions.ILike(p.Especie.NomeVulgar, $"%{termoDeBusca}%") || 
                    EF.Functions.ILike(p.Especie.Taxon, $"%{termoDeBusca}%")
                );
            }

            // Executo a query no banco de dados (só agora é que o Postgres trabalha)
            var peixesNaBaseDeDados = await query.ToListAsync();
            
            var resultado = new List<PeixeResponseDTO>();

            // Mini Factory Transformando as entidades em DTOs
            foreach (var item in peixesNaBaseDeDados)
            {
                // Descobrindo o preço atual do peixe (igual ao Carrinho)
                var precoTabela = item.Especie.Precos.FirstOrDefault(p => 
                    p.EstadoSaudeId == item.EstadoSaudeId && 
                    p.EstadoDesenvolvimentoId == item.EstadoDesenvolvimentoId);

                decimal preco = precoTabela?.Valor ?? 0;
                string nome = item.Especie.NomeVulgar ?? item.Especie.Taxon;
                //Verificar se devo abandonar este snipet.
                //Tô adicionando aqui o código de service inteligente de nomes de imagem:
                string nomeArquivoImagem = item.Especie.NomeVulgar.ToLower().Replace(" ", "-") + ".png";


                resultado.Add(new PeixeResponseDTO(
                    item.Id,
                    item.LoteId,
                    item.EspecieId,
                    item.EstadoSaudeId,
                    item.EstadoDesenvolvimentoId,
                    nome,
                    item.Sexo ?? "N/D",
                    preco,
                    item.Especie.ImagemUrl
                ));
            }
            
            return resultado;
        }
    }
}
