using System.ComponentModel.DataAnnotations.Schema;

namespace API_DB_PESCES_em_C__bonitona.Models
{
    [Table("carrinhos")]
    public class Carrinho
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("data_criacao")]
        public DateTime DataCriacao {get; set;}

        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<ItemCarrinho> Itens { get; set; } = []; // = new List<ItemCarrinho>();
        
    }
}