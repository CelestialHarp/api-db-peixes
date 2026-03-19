
namespace API_DB_PESCES_em_C__bonitona.Models;

public partial class Lote
{
    public int Id { get; set; }

    public int? Status { get; set; }

    public string Descricao { get; set; } = null!;

    public int? QuantidadePeixes { get; set; }

    public decimal? PrecoLote { get; set; }

    public virtual ICollection<Pesce> Pesces { get; set; } = new List<Pesce>();
}
