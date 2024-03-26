using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerID.Models
{
    [Table("Categories")]
    public class Category : BaseModel
    {
        [Column("category_id")]
        [MaxLength(26)]
        public string Id { get; set; }
        [Column("category_name")]
        [MaxLength(200)]
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
