using EmpowerID.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpowerID.Models
{
    public abstract class BaseModel : IBaseModel
    {
        //[NotMapped]
        [Column("DataStatus")]
        public virtual DataStatus DataStatus { set; get; }
    }
}
