namespace API_DB_PESCES_em_C__bonitona.Models;

public partial class Comportamento
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public virtual ICollection<Especie> Especies { get; set; } = new List<Especie>();
}
