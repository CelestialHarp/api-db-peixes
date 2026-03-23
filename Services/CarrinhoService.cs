
using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Enums;
using API_DB_PESCES_em_C__bonitona.Models;
using Microsoft.EntityFrameworkCore;

namespace API_DB_PESCES_em_C__bonitona.Services
{
    public class CarrinhoService
    {
        private readonly DbPescesContext _context;

        public CarrinhoService(DbPescesContext context)
        {
            _context = context;
        }

       
        //Isso serve pra "relembrar" a UI de quais itens o usuário possui em seu carrinho, uma vez que o usuário saia e volte à página. Este método também assegura uma fonte única de verdade, que impede que sejam performadas operações sobre os itens segundo valores antigos e/ou desatualizados. A informação-verdade simplesmente não deve ser transferida à guarda da UI por causa da possibilidade de amnésia e de manipulação de dados.
        public async Task<CarrinhoResponseDTO> ObterCarrinhoAsync(int usuarioId)
        {
            var carrinho = 
            await _context.Carrinhos
            .AsSplitQuery()
            .Include(c => c.Itens)
            .ThenInclude(i => i.Pesce)
            .ThenInclude(p => p.Especie)
            .ThenInclude(e => e.Precos) // Pegando preço para calcular o total
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null)
            {
                //Se não há carrinho, provê-se um carrinho vazio.
                return new CarrinhoResponseDTO(0, usuarioId, 0, []);
            }

            return MapearParaDTO(carrinho);
        }

        // Adicionar Peixe ao Carrinho
        public async Task<CarrinhoResponseDTO> AdicionarItemAsync(int usuarioId, int pesceId)
        {
            // Validações de Segurança e Concorrência
            var peixeExiste = await _context.Peixes.AnyAsync(p => p.Id == pesceId);
            if (!peixeExiste) throw new Exception("Peixe não encontrado no sistema.");

            var peixeEmOutroCarrinho = await _context.ItensCarrinho.AnyAsync(i => i.PesceId == pesceId);
            if (peixeEmOutroCarrinho) throw new Exception("Lamentamos, mas este peixe já está no carrinho de outro cliente.");

            var peixeJaVendido = await _context.ItensPedido.AnyAsync(i => i.PesceId == pesceId);
            if (peixeJaVendido) throw new Exception("Este peixe já foi vendido.");

            // Procura o carrinho do usuário e se não existir cria um novo
            var carrinho = await _context.Carrinhos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
            
            if (carrinho == null)
            {
                carrinho = new Carrinho { UsuarioId = usuarioId };
                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync(); 
            }

            // Adiciona o peixe ao carrinho
            var novoItem = new ItemCarrinho
            {
                CarrinhoId = carrinho.Id,
                PesceId = pesceId
            };

            _context.ItensCarrinho.Add(novoItem);
            await _context.SaveChangesAsync();

            // Devolve o carrinho, agora atualizado com o novo item
            return await ObterCarrinhoAsync(usuarioId);
        }

        public async Task<CarrinhoResponseDTO> RemoverItemAsync(int usuarioId, int pesceId)
        {
            // Depois vou procurar qual é a convenção em C# para comentários que já foram feitos, para ver se cai bem explicar snipets que já foram explicados antes, pra facilitar a vida de que der um peek implementation
            var carrinho = await _context.Carrinhos.Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
            //Acho que isso aqui é inútil, já que não vai ter botão de excluir item se o cara nem sequer tiver carrinho. Sob condições normais, eu retiraria esse snipet, mas preciso ver se por acaso ele é invocado na UI como condição para o travamento da página de carrinho, o muito provavelmente não acontece, mas eu tenho que checar de qualquer modo. 
            if (carrinho == null)
            {
                throw new Exception("Você não possui um carrinho ativo.");
            }

            var itemParaRemover = carrinho.Itens.FirstOrDefault(i => i.PesceId == pesceId);

            //provavelmente deve ter um pouquinho de lógica de checking duplicada, espalhada em umas 2 ou 3 classes, que acabei implementando por medo de dar algo errado, então depois vou pesquisar qual seria a melhor prática.
            if (itemParaRemover == null) throw new Exception("Este item não está no seu carrinho.");
            
            _context.ItensCarrinho.Remove(itemParaRemover);
            await _context.SaveChangesAsync();

            // Devolve o carrinho, agora atualizado com o novo item
            return await ObterCarrinhoAsync(usuarioId);
        }

        // 3. Função Auxiliar para montar a Resposta e Calcular os Preços
        private static CarrinhoResponseDTO MapearParaDTO(Carrinho carrinho)
        {
            var itensDTO = new List<ItemCarrinhoResponseDTO>();
            decimal total = 0;

            foreach (var item in carrinho.Itens)
            {
                if (item.Pesce == null || item.Pesce.Especie == null) continue;

                // Encontra o preço exato com base na espécie, saúde e desenvolvimento atual do peixe
                var precoTabela = item.Pesce.Especie.Precos.FirstOrDefault(p => 
                    p.EstadoSaudeId == item.Pesce.EstadoSaudeId && 
                    p.EstadoDesenvolvimentoId == item.Pesce.EstadoDesenvolvimentoId);

                decimal preco = precoTabela?.Valor ?? 0;

                string nome = item.Pesce.Especie.NomeVulgar ?? item.Pesce.Especie.Taxon;

                string imagem = item.Pesce.Especie.ImagemUrl;

                itensDTO.Add(new ItemCarrinhoResponseDTO(item.Id, item.PesceId, nome, preco, imagem));

                total += preco;
            }

            return new CarrinhoResponseDTO(carrinho.Id, carrinho.UsuarioId, total, itensDTO);
        }
    }
}