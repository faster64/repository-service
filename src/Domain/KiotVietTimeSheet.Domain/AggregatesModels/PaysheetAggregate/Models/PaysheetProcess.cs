
using KiotVietTimeSheet.SharedKernel.Models;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models
{
    public class PaysheetProcess : BaseEntity
    {
        /// <summary>
        /// Mã bảng lương
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// Id cửa hàng/doanh nghiệp
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Id chi nhánh
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// Thời điểm tạo process
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Thời điểm tạo process
        /// </summary>
        public long PaySheetId { get; set; }

        /// <summary>
        /// Trạng thái của process
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// thanh  phần của process
        /// </summary>
        public int ParticipantsComplete { get; set; }
    }
}
