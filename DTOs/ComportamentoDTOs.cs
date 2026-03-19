namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    public record CreateComportamentoDTO
    (
        string Nome,
        string Descricao
    );   
    public record ResponseComportamentoDTO
    (
        int Id,
        string Nome,
        string Descricao
    );
}