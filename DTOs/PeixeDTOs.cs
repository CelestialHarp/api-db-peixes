
namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    public record CreatePeixeDTO
    (
        int EspecieId,
        int? LoteId, //O peixe não nasce vinculado a um lote, ele pode existir sozinho, sem lote. 
        int EstadoDeSaudeId,
        int EstadoDeDesenvolvimentoId,
        string Sexo
    );
    //Alterei o PeixeResponse porque queria dar mais informação.
    public record PeixeResponseDTO
    (
        int Id,
        int? LoteId,
        int EspecieId,
        int EstadoDeSaudeId,
        int EstadoDeDesenvolvimentoId,
        string NomeEspecie, //Anotação minha aparentemente obsoleta que preciso checar: Substituir por NomeVulgar, e adicionar atributo de taxon
        string Sexo,
        decimal PrecoCalculado, // O preço que vem daquela tabela complexa
        string? ImagemUrl = null
    );
}