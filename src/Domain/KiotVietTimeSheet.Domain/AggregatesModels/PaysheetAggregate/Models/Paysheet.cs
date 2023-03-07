using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models
{
    public class Paysheet : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ICode,
        ITenantId,
        IBranchId,
        ISoftDelete,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        IIsDraft
    {
        public static string CodePrefix = "BL"; //NOSONAR
        public static string CodeDelSuffix = "{DEL"; //NOSONAR
        public static string CodeDraftSuffix = "{Draft}"; //NOSONAR

        #region Properties
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
        /// Kết thúc kì làm việc
        /// </summary>
        public DateTime EndTime { get; set; }

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
        public List<Payslip> Payslips { get; set; } = new List<Payslip>();

        /// <summary>
        /// Ngày công chuẩn 
        /// </summary>
        public int WorkingDayNumber { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

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
        /// Tên kỳ lương
        /// </summary>
        public string PaysheetPeriodName { get; protected set; }

        /// <summary>
        /// Người lập bảng lương
        /// </summary>
        public long CreatorBy { get; protected set; }

        /// <summary>
        /// Thời điểm tạo bảng lương
        /// </summary>
        public DateTime? PaysheetCreatedDate { get; set; }

        /// <summary>
        /// Dùng để xác định trạng thái của một bảng lương khi có sự thay đổi dữ liệu chấm công, mức lương và cài đặt
        /// </summary>
        public long Version { get; set; }

        public bool IsDraft { get; set; }

        /// <summary>
        /// Số giờ làm việc của 1 ngày công chuẩn
        /// </summary>
        public int? TimeOfStandardWorkingDay { get; set; }

        public int? ErrorStatus { get; set; }
        #endregion

        #region Constructors
        // For Persistence
        public Paysheet()
        {

        }

        [JsonConstructor]
        public Paysheet(
            long id,
            string code,
            int tenantId,
            int branchId,
            bool isDeleted,
            long? deletedBy,
            DateTime? deletedDate,
            long createdBy,
            DateTime createdDate,
            string name,
            byte salaryPeriod,
            DateTime startTime,
            DateTime endTime,
            byte paysheetStatus,
            string note,
            List<Payslip> payslips,
            int workingDayNumber,
            long? modifiedBy,
            DateTime? modifiedDate,
            string paysheetPeriodName,
            long creatorBy
        )
        {
            Id = id;
            Code = code;
            TenantId = tenantId;
            BranchId = branchId;
            IsDeleted = isDeleted;
            DeletedBy = deletedBy;
            DeletedDate = deletedDate;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            Name = name;
            SalaryPeriod = salaryPeriod;
            StartTime = startTime;
            EndTime = endTime;
            PaysheetStatus = paysheetStatus;
            Note = note;
            Payslips = payslips;
            WorkingDayNumber = workingDayNumber;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            PaysheetPeriodName = paysheetPeriodName;
            CreatorBy = creatorBy;
        }

        public Paysheet(
            string code,
            string note,
            int workingDayNumber,
            byte salaryPeriod,
            DateTime startTime,
            DateTime endTime,
            byte paysheetStatus,
            long creatorBy,
            bool isDraft,
            int timeOfStandardWorkingDay
            )
        {
            var paysheetPeriodName = startTime.ToString("dd/MM/yyyy") + " - " + endTime.ToString("dd/MM/yyyy");

            Code = code;
            Name = "Bảng lương " + paysheetPeriodName;
            Note = note;
            WorkingDayNumber = workingDayNumber;
            SalaryPeriod = salaryPeriod;
            StartTime = startTime.Date;
            EndTime = endTime.Date.AddDays(1).AddSeconds(-1);
            PaysheetStatus = paysheetStatus;
            PaysheetPeriodName = paysheetPeriodName;
            CreatorBy = creatorBy;
            PaysheetCreatedDate = DateTime.Now;
            IsDraft = isDraft;
            TimeOfStandardWorkingDay = timeOfStandardWorkingDay;
        }
        #endregion

        public void Update(
            string code,
            string name,
            string note,
            int workingDayNumber,
            byte salaryPeriod,
            DateTime startTime,
            DateTime endTime,
            string paysheetPeriodName,
            long creatorBy,
            DateTime? paysheetCreatedDate,
            byte paysheetStatus
            )
        {
            Code = code;
            Name = name;
            Note = note;
            WorkingDayNumber = workingDayNumber;
            SalaryPeriod = salaryPeriod;
            StartTime = startTime.Date;
            EndTime = endTime.Date.AddDays(1).AddSeconds(-1);
            PaysheetPeriodName = paysheetPeriodName ?? string.Empty;
            CreatorBy = creatorBy;
            PaysheetCreatedDate = paysheetCreatedDate;
            PaysheetStatus = paysheetStatus;
            Version = 0;

            IsDraft = false;
        }

        public void UpdatePeriodName(string paysheetPeriodName)
        {
            PaysheetPeriodName = paysheetPeriodName ?? string.Empty;
        }

        public void UpdateStartAndEndTime(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public void Cancel()
        {
            var beforeChange = (Paysheet)MemberwiseClone();
            PaysheetStatus = (byte)PaysheetStatuses.Void;
            AddDomainEvent(new CancelPaysheetEvent(beforeChange));
        }

        public void Complete()
        {
            PaysheetStatus = (byte)PaysheetStatuses.PaidSalary;
            Payslips?.ForEach(payslip => payslip.Complete());
        }

        public void ResetVersion()
        {
            Version = 0;
        }

        public void CalculateTotalNeedPay(decimal totalNetSalary, decimal totalPayment)
        {
            TotalNeedPay =  totalNetSalary - totalPayment > 0 ? totalNetSalary - totalPayment : 0;
        }
    }
}
