
namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    public record CreatePrecoDTO
    (
        decimal Valor,
        int EspecieId,
        int EstadoDeSaudeId,
        int EstadoDeDesenvolvimentoID
    );

    public record ResponsePrecoDTO
    (
        int Id,
        decimal Valor,
        string EspecieNome,
        string SaudeDescricao,
        string DesenvolvimentoDescricao
    );
}