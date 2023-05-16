
using System.ComponentModel.DataAnnotations.Schema;

namespace BBGPunchService.Core.Model.SourceEntity
{
    [Table("Employee")]
    public class Employee :BaseEntity
    {
        public string? EmployeeName { get; set; }

        public string? EnrollNo { get; set; }
      
        public virtual ICollection<Punch>? Punch { get; set; }
    }

}
