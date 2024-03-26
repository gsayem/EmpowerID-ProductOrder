using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerID.Models
{
    [Table("Orders")]
    public class Order : BaseModel
    {
        [Column("order_id")]
        [MaxLength(26)]
        public string Id { get; set; }
        [Column("order_date")]
        public DateTime OrderDate { get; set; }
        [Column("customer_name")]
        [MaxLength(200)]
        public string CustomerName { get; set; }

        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
