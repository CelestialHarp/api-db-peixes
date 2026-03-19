namespace API_DB_PESCES_em_C__bonitona.Models;

public partial class EstadoDesenvolvimento
{
    public int Id { get; set; }

    public string Descricao { get; set; } = null!;

    public virtual ICollection<Pesce> Pesces { get; set; } = new List<Pesce>();

    public virtual ICollection<Preco> Precos { get; set; } = new List<Preco>();
}
