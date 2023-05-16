using System.ComponentModel.DataAnnotations.Schema;

namespace BBGPunchService.Core.Model.TargetEntity
{
    [Table("PunchingData")]
    public class PunchingData:BaseEntity
    {
        public string? EnrolNo { get; set; }
        public string? FullName { get; set; }
        public DateTime? PunchDateTime { get; set; }
        public string? PunchDirection { get; set; }
        public int? PunchNumber { get; set; }
    }
}
