
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
            .Include(c => c.Items)
            .ThenInclude(i => i.Peixe)
            .ThenInclude(p => p.Especie)
            .ThenInclude(e => e.Precos)
            .FirstOrDefaultAsync(c => c.UserId == usuarioId);

            if (carrinho == null || !carrinho.Items.Any())
            {
                throw new Exception("Seu carrinho está vazio");
            }
            
            var pedido = new Order
            {
                UserId = usuarioId,
                ValorTotal = 0,
                Status = StatusPedido.Confirmado,
                Items = [] // Prepara a gaveta vazia de itens (usando o atalho)
            };

            var itensDTO = new List<ItemPedidoResponseDTO>();

            foreach (var item in carrinho.Items)
            {
                if (item.Peixe == null || item.Peixe.Especie == null) continue;

                // Encontra o preço exato com base na espécie, saúde e desenvolvimento atual do peixe
                var precoTabela = item.Peixe.Especie.Precos.FirstOrDefault
                (p => 
                    p.EstadoSaudeId == item.Peixe.HealthStateId &&
                    p.EstadoDesenvolvimentoId == item.Peixe.EstadoDesenvolvimentoId
                );

                decimal preco = precoTabela?.Valor ?? throw new Exception("Programação defensiva. Se, de algum modo, alguém burlar a vitrine, dá nisso.");

                pedido.Items.Add(new ItemPedido { PeixeId = item.PeixeId, PrecoNoMomento = preco }); 
                pedido.ValorTotal += preco;

                string nome = item.Peixe.Especie.NomeVulgar ?? item.Peixe.Especie.Taxon;
                itensDTO.Add(new ItemPedidoResponseDTO(0, item.PeixeId, nome, preco));
            }

            _context.Pedidos.Add(pedido);
            _context.ItensCarrinho.RemoveRange(carrinho.Items);
            await _context.SaveChangesAsync();

            return new PedidoResponseDTO(pedido.Id, pedido.UserId, pedido.ValorTotal, pedido.Status, itensDTO);


        }


    }
}