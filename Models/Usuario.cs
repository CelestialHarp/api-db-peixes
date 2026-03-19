
using System.ComponentModel.DataAnnotations.Schema;

namespace API_DB_PESCES_em_C__bonitona.Models
{
    [Table("usuarios")] //indicando o nome real da tabela
    public class Usuario
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; } = string.Empty;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("cargo")]
        public string Cargo { get; set; } = string.Empty;
    }
}