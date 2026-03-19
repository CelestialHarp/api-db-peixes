
namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    public record AdicionarItemCarrinhoDTO(
        int PesceId
    );
    public record ItemCarrinhoResponseDTO(
        int Id, 
        int PesceId, 
        string EspecieNome, 
        decimal Preco,
        string? ImagemUrl = null
    );

    public record CarrinhoResponseDTO(
        int Id, 
        int UsuarioId, 
        decimal TotalCarrinho, 
        List<ItemCarrinhoResponseDTO> Itens
    );
}