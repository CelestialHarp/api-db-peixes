namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    public record CreateLoteDTO
    (
        string Descricao
    );
    public record FinishLoteDTO
    (
        int LoteId,
        decimal PrecoFinal
    );

    public record LoteResponseDTO
    (
        int Id,
        string Descricao,
        int QuantidadePeixes,
        List<PeixeResponseDTO> Peixes,
        decimal PrecoFinal,
        decimal SugestaoPreco
    );
}