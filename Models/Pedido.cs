
using System.ComponentModel.DataAnnotations.Schema;
using API_DB_PESCES_em_C__bonitona.Enums;

namespace API_DB_PESCES_em_C__bonitona.Models
{
    [Table("pedidos")]
    public class Pedido
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("data_pedido")]
        public DateTime DataCriacao {get; set;}

        [Column("valor_total")]
        public decimal ValorTotal { get; set; }

        [Column("status")]
        public StatusPedido Status { get; set; } = StatusPedido.Confirmado;

        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<ItemPedido> Itens { get; set; } = []; //= new List<ItemPedido>(); 
        
    }
}