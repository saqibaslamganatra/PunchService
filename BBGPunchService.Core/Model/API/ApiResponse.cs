using BBGPunchService.Core.Model.TargetEntity;

namespace BBGPunchService.Core.Model.API
{
    public class ApiResponse
    {
        public List<PunchingData> Result { get; set; }
        public PaginationInfo Pagination { get; set; }
    }
}
