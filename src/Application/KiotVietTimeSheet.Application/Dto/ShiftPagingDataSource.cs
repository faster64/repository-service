using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class HolidayPagingDataSource : PagingDataSource<HolidayDto>
    {
        public int TotalDays { get; set; }
    }
}
