using EmpowerID.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerID.Models.CDC
{
    public class CDC_Order : BaseModel
    {

        [Column("DataStatus")]
        public DataStatus DataStatus { set; get; }
        [Column("order_id")]
        [MaxLength(26)]
        public string Id { get; set; }
        [Column("order_date")]
        public DateTime OrderDate { get; set; }
        [Column("customer_name")]
        [MaxLength(200)]
        public string CustomerName { get; set; }
    }
}
