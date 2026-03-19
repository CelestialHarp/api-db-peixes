
using System.ComponentModel.DataAnnotations.Schema;


namespace API_DB_PESCES_em_C__bonitona.Models
{
    [Table("guildas_troficas")]
    public class GuildaTrofica
    {
        [Column("id")]
        public int Id {get; set;}

        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
        
    }
}