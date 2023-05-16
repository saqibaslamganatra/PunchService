using System.ComponentModel.DataAnnotations.Schema;

namespace BBGPunchService.Core.Model.SourceEntity
{
    [Table("Punch")]
    public class Punch:BaseEntity
    {
        public DateTime? PunchTime { get; set; }

        [Column("Employee")]
        [ForeignKey("Employee")]
        public Guid? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }        
        public int? PunchType { get; set; }
        public int? PunchNumber { get; set; }

    }
}
