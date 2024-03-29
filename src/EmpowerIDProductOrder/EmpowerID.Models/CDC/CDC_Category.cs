using EmpowerID.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerID.Models.CDC
{
    public class CDC_Category : BaseModel
    {

        [Column("DataStatus")]
        public DataStatus DataStatus { set; get; }
        [Column("category_id")]
        [MaxLength(26)]
        public string Id { get; set; }
        [Column("category_name")]
        [MaxLength(200)]
        public string Name { get; set; }
    }
}
