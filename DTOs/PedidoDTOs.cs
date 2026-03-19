using API_DB_PESCES_em_C__bonitona.Enums;
namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    //Como o pedido é basicamente um carrinho, então fiz com base nos DTOS de carrinho
    public record ItemPedidoResponseDTO(
        int Id, 
        int PesceId, 
        string EspecieNome, 
        decimal Preco
    );

    public record PedidoResponseDTO(
        int Id, 
        int UsuarioId, 
        decimal TotalPedido,
        StatusPedido Status,
        List<ItemPedidoResponseDTO> Itens
    );
}