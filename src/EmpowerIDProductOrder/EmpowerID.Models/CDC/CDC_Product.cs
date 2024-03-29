using EmpowerID.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerID.Models.CDC
{
    public class CDC_Product : BaseModel
    {
        [Column("DataStatus")]
        public DataStatus DataStatus { set; get; }

        [Column("product_id")]
        [MaxLength(26)]
        public string Id { get; set; }
        [Column("product_name")]
        [MaxLength(200)]
        public string Name { get; set; }
        [Column("category_id")]
        [MaxLength(26)]
        public string CategoryId { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("image_url")]
        [MaxLength(400)]
        public string ImageURL { get; set; }

        [Column("date_added")]
        public DateTime DateAddded { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

    }
}
