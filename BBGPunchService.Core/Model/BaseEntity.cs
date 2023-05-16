using System.ComponentModel.DataAnnotations;

namespace BBGPunchService.Core.Model
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Oid { get; set; }
    }
}
