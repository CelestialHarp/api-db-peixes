using System.ComponentModel.DataAnnotations.Schema;

namespace API_DB_PESCES_em_C__bonitona.Models;

public partial class Especie
{
    public int Id { get; set; }

    public string? NomeVulgar { get; set; }

    public string Taxon { get; set; } = null!;

    public string? Subespecie { get; set; }

    public string? Linhagem { get; set; }

    public string? ImagemUrl { get; set; }

    public string? Comentario { get; set; }

    public int? ComportamentoId { get; set; }

    [Column("guilda_trofica_id")]
    public int GuildaTroficaId { get; set; }

    //lembrar de perguntar como funciona essa "ponte de navegação virtual"
    public virtual Comportamento? Comportamento { get; set; }

    public virtual GuildaTrofica? GuildaTrofica { get; set; }

    public virtual ICollection<Pesce> Pesces { get; set; } = new List<Pesce>();

    public virtual ICollection<Preco> Precos { get; set; } = new List<Preco>();
}
