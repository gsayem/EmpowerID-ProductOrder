

//Not in the Requirements

//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace EmpowerID.Models
//{

//    public class ProductOrder : BaseModel
//    {
//        [Key]
//        [Column("product_order_id")]
//        [MaxLength(26)]
//        public string Id { get; set; }
//        [Column("product_id")]
//        [MaxLength(26)]
//        public string ProductId { get; set; }
//        [Column("order_id")]
//        [MaxLength(26)]
//        public string OrderId { get; set; }

//        [ForeignKey(nameof(ProductId))]
//        public virtual Product Product { get; set; }

//        [ForeignKey(nameof(OrderId))]
//        public virtual Order Order { get; set; }
//    }
//}
