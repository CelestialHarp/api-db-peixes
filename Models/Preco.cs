namespace API_DB_PESCES_em_C__bonitona.Models;

public partial class Preco
{
    public int Id { get; set; }

    public decimal? Valor { get; set; }

    public int? EspecieId { get; set; }

    public int? EstadoSaudeId { get; set; }

    public int? EstadoDesenvolvimentoId { get; set; }

    public virtual Especie? Especie { get; set; }

    public virtual EstadoDesenvolvimento? EstadoDesenvolvimento { get; set; }

    public virtual EstadoSaude? EstadoSaude { get; set; }
}
