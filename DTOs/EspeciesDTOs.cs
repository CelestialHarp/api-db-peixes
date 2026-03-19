

namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    public record CreateEspecieDTO
    (
        string? NomeVulgar,
        string Taxon,
        string? Subespecie,
        string? Linhagem,
        string? ImagemUrl,
        string? Comentario,
        int ComportamentoId,
        int GuildaTroficaId
    );
    public record ResponseEspecieDTO(
    int Id,
    string NomeVulgar,
    string Taxon,
    string? Subespecie,
    string? Linhagem,
    string? ImagemUrl,
    string? Comentario,
    string NomeComportamento,
    string GuildaTrofica
    );
}