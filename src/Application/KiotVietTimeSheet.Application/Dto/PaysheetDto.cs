using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PaysheetDto
    {
        /// <summary>
        /// Id bảng lương
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Mã bảng lương
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id cửa hàng/doanh nghiệp
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Id chi nhánh
        /// </summary>
        public int BranchId { get; set; }
        public string BranchName { get; set; }

        /// <summary>
        /// Trạng thái xóa của bảng lương
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Id người thực hiện xóa bảng lương
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Thời điểm xóa bảng lương
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Người tạo bảng lương
        /// </summary>
        public long CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        /// <summary>
        /// Thời điểm tạo bảng lương
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Tên bảng lương
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// chu kì trả lương của bảng lương
        /// </summary>
        public byte SalaryPeriod { get; set; }

        /// <summary>
        /// Bắt đầu kì làm việc
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Trạng thái bảng lương <see cref="PaysheetStatuses"/>
        /// </summary>
        public byte PaysheetStatus { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Danh sách các phiếu lương trong bảng lương
        /// </summary>
        public List<PayslipDto> Payslips { get; set; }

        /// <summary>
        /// Ngày công chuẩn 
        /// </summary>
        public int WorkingDayNumber { get; set; }

        /// <summary>
        /// Tên kỳ lương
        /// </summary>
        public string PaysheetPeriodName { get; set; }
        public DateTime EndTime { get; set; }

        private long? _employeeTotal;
        /// <summary>
        /// Tổng số nhân viên của bảng lương
        /// </summary>
        public long EmployeeTotal
        {
            get
            {
                if (!_employeeTotal.HasValue && Payslips != null)
                    _employeeTotal = Payslips.GroupBy(p => p.EmployeeId).Select(p => p.Key).Count();

                return _employeeTotal ?? 0;
            }
            set => _employeeTotal = value;
        }

        private decimal? _totalNetSalary;
        /// <summary>
        /// Tổng số tiền thực lĩnh của bảng lương
        /// </summary>
        public decimal TotalNetSalary
        {
            get
            {
                if (!_totalNetSalary.HasValue && Payslips != null)
                    _totalNetSalary = Payslips.Where(x => !x.IsDeleted && x.PayslipStatus != (byte)PaysheetStatuses.Void).Sum(p => p.NetSalary);

                return _totalNetSalary ?? 0;
            }
            set => _totalNetSalary = value;
        }



        private decimal? _totalPayment;
        /// <summary>
        /// Tổng số tiền đã trả của bảng lương
        /// </summary>
        public decimal TotalPayment
        {
            get
            {
                if (!_totalPayment.HasValue && Payslips != null)
                    _totalPayment = Payslips.Sum(p => p.TotalPayment);

                return _totalPayment ?? 0;
            }
            set => _totalPayment = value;
        }

        private decimal? _totalNeedPay;
        /// <summary>
        /// Còn cần trả
        /// </summary>
        public decimal TotalNeedPay
        {
            get
            {
                if (_totalNeedPay.HasValue) return _totalNeedPay.Value;
                _totalNeedPay = TotalNetSalary - TotalPayment;
                _totalNeedPay = _totalNeedPay > 0 ? _totalNeedPay : 0;
                return _totalNeedPay.Value;
            }
            set => _totalNeedPay = value;
        }

        /// <summary>
        /// Người lập bảng lương
        /// </summary>
        public long CreatorBy { get; set; }
        public string CreatorByName { get; set; }

        public bool IsChanged { get; set; }

        public int TimeOfStandardWorkingDay { get; set; }

        /// <summary>
        /// Thời điểm tạo bảng lương
        /// </summary>
        public DateTime? PaysheetCreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long Version { get; set; }

        public bool IsDraft { get; set; }

        public List<long> ClockingIds { get; set; }
        public int? ErrorStatus { get; set; }
    }
}
