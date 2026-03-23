
using System.ComponentModel.DataAnnotations.Schema;


namespace API_DB_PESCES_em_C__bonitona.Models
{
    [Table("itens_carrinho")]
    public class ItemCarrinho
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("carrinho_id")]
        public int CarrinhoId { get; set; }

        [Column("pesce_id")]
        public int PesceId { get; set; }

        public virtual Carrinho? Carrinho { get; set; }
        public virtual Peixe? Pesce { get; set; }
    }
}