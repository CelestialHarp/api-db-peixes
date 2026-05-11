using System.ComponentModel.DataAnnotations.Schema;

namespace API_DB_PESCES_em_C__bonitona.Models
{
    [Table("carrinhos")]
    public class Carrinho
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UserId { get; set; }

        [Column("data_criacao")]
        public DateTime DataCriacao {get; set;}

        public virtual Usuario? User { get; set; }
        public virtual ICollection<ItemCarrinho> Items { get; set; } = []; // = new List<ItemCarrinho>();
        
    }
}