
using API_DB_PESCES_em_C__bonitona.DTOs;
using API_DB_PESCES_em_C__bonitona.Enums;
using API_DB_PESCES_em_C__bonitona.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace API_DB_PESCES_em_C__bonitona.Services
{
    public class PedidoService
    {
        private readonly DbPescesContext _context;

        // O mesmo negócio de sugestão de "Use primary constructor"
        public PedidoService(DbPescesContext context)
        {
            _context = context;
        }


        public async Task<PedidoResponseDTO> FinalizarCompraAsync (int usuarioId){
            var carrinho = 
            await _context.Carrinhos
            .AsSplitQuery()
            .Include(c => c.Itens)
            .ThenInclude(i => i.Pesce)
            .ThenInclude(p => p.Especie)
            .ThenInclude(e => e.Precos)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null || !carrinho.Itens.Any())
            {
                throw new Exception("Seu carrinho está vazio");
            }
            
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                ValorTotal = 0,
                Status = StatusPedido.Confirmado,
                Itens = [] // Prepara a gaveta vazia de itens (usando o atalho)
            };

            var itensDTO = new List<ItemPedidoResponseDTO>();

            foreach (var item in carrinho.Itens)
            {
                if (item.Pesce == null || item.Pesce.Especie == null) continue;

                // Encontra o preço exato com base na espécie, saúde e desenvolvimento atual do peixe
                var precoTabela = item.Pesce.Especie.Precos.FirstOrDefault
                (p => 
                    p.EstadoSaudeId == item.Pesce.EstadoSaudeId &&
                    p.EstadoDesenvolvimentoId == item.Pesce.EstadoDesenvolvimentoId
                );

                decimal preco = precoTabela?.Valor ?? throw new Exception("Programação defensiva. Se, de algum modo, alguém burlar a vitrine, dá nisso.");

                pedido.Itens.Add(new ItemPedido { PesceId = item.PesceId, PrecoNoMomento = preco }); 
                pedido.ValorTotal += preco;

                string nome = item.Pesce.Especie.NomeVulgar ?? item.Pesce.Especie.Taxon;
                itensDTO.Add(new ItemPedidoResponseDTO(0, item.PesceId, nome, preco));
            }

            _context.Pedidos.Add(pedido);
            _context.ItensCarrinho.RemoveRange(carrinho.Itens);
            await _context.SaveChangesAsync();

            return new PedidoResponseDTO(pedido.Id, pedido.UsuarioId, pedido.ValorTotal, pedido.Status, itensDTO);


        }


    }
}