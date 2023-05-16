namespace BBGPunchService.Api.Dto
{
    public class PunchDataDto
    {
        public int pageSize { get; set; } = 10;
        public int pageNumber { get; set; } = 1;
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public string? searchText { get; set; }
    }

}
