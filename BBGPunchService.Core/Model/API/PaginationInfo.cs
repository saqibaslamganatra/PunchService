﻿
namespace BBGPunchService.Core.Model.API
{
    public class PaginationInfo
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
