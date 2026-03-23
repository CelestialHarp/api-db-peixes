
using System.ComponentModel.DataAnnotations.Schema;

namespace API_DB_PESCES_em_C__bonitona.Models
{
    [Table("itens_pedido")]
    public class ItemPedido
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("pedido_id")]
        public int PedidoId { get; set; }

        [Column("pesce_id")]
        public int PesceId { get; set; }

        [Column("preco_no_momento")]
        public decimal PrecoNoMomento { get; set; }

        public virtual Pedido? Pedido { get; set; }
        public virtual Peixe? Pesce { get; set; }
    }
}