namespace API_DB_PESCES_em_C__bonitona.Models;

public partial class Peixe
{
    public int Id { get; set; }

    public string? Sexo { get; set; }

    public DateOnly? DataNascimento { get; set; }

    public int EspecieId { get; set; }

    public int EstadoSaudeId { get; set; }

    public int EstadoDesenvolvimentoId { get; set; }

    public int? LoteId { get; set; }

    public virtual Especie Especie { get; set; } = null!;

    public virtual EstadoDesenvolvimento EstadoDesenvolvimento { get; set; } = null!;

    public virtual EstadoSaude EstadoSaude { get; set; } = null!;

    public virtual Lote? Lote { get; set; }
}
