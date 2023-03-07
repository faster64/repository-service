using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models
{
    public class Shift : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        IBranchId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete,
        ICacheable
    {
        #region Constructor
        public Shift()
        {
            IsActive = true;
            IsDeleted = false;
        }

        [JsonConstructor]
        public Shift(long id, string name, long from, long to, bool isActive, int branchId)
        {
            Id = id;
            Name = name;
            From = from;
            To = to;
            IsActive = isActive;
            BranchId = branchId;
        }

        /// <summary>
        /// Tạo mới ca làm việc từ các tham số truyền vào 
        /// </summary>
        /// <param name="name">Tên ca làm việc</param>
        /// <param name="from">Thời gian bắt đầu</param>
        /// <param name="to">Thời gian kết thúc</param>
        /// <param name="isActive">Trạng thái hoạt động</param>
        /// <param name="branchId">Id của chi nhánh áp dụng kỳ nghỉ</param>
        /// <param name="checkInBefore">Thời gian cho phép chấm vào</param>
        /// <param name="checkOutAfter">Thời gian cho phép chấm ra</param>
        /// <returns></returns>
        public Shift(AddShift addShift)
        {
            Name = (addShift.Name ?? string.Empty).ToPerfectString();
            From = addShift.From;
            To = addShift.To;
            IsActive = addShift.IsActive;
            BranchId = addShift.BranchId;
            CheckInBefore = addShift.CheckInBefore;
            CheckOutAfter = addShift.CheckOutAfter;
            AddDomainEvent(new CreatedShiftEvent(this, addShift.IsGeneralSetting));
        }

        // Only copy primitive values
        public Shift(Shift shift)
        {
            Id =  shift.Id;
            Name =  shift.Name;
            From =  shift.From;
            To =  shift.To;
            IsActive =  shift.IsActive;
            IsDeleted =  shift.IsDeleted;
            BranchId =  shift.BranchId;
            TenantId =  shift.TenantId;
            CheckInBefore = shift.CheckInBefore;
            CheckOutAfter = shift.CheckOutAfter;
            DeletedBy =  shift.DeletedBy;
            DeletedDate =  shift.DeletedDate;
            CreatedBy =  shift.CreatedBy;
            CreatedDate =  shift.CreatedDate;
            ModifiedBy =  shift.ModifiedBy;
            ModifiedDate =  shift.ModifiedDate;
            if (shift.DomainEvents != null)
            {
                foreach (var domainEvent in shift.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion

        #region Properties
        public long Id { get; set; }
        public string Name { get; protected set; }
        public long From { get; protected set; }
        public long To { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool IsDeleted { get; set; }
        public int BranchId { get; set; }
        public long? CheckInBefore { get; protected set; }
        public long? CheckOutAfter { get; protected set; }
        public int TenantId { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Cập nhật ca làm việc từ các tham số truyền vào
        /// </summary>
        /// <param name="name">Tên ca làm việc</param>
        /// <param name="from">Thời gian bắt đầu</param>
        /// <param name="to">Thời gian kết thúc</param>
        /// <param name="isActive">Trạng thái hoạt động</param>
        /// <param name="checkInBefore">Thời gian cho phép chấm vào</param>
        /// <param name="checkOutAfter">Thời gian cho phép chấm ra</param>
        /// <returns></returns>
        public void Update(string name, long from, long to, bool isActive, long checkInBefore, long checkOutAfter, bool isGeneralSetting = false)
        {
            var beforeChange = MemberwiseClone() as Shift;

            Name = (name ?? string.Empty).Trim();
            From = from;
            To = to;
            IsActive = isActive;
            CheckInBefore = checkInBefore;
            CheckOutAfter = checkOutAfter;
            AddDomainEvent(new UpdatedShiftEvent(beforeChange, this, isGeneralSetting));
        }

        public void Delete(bool isGeneralSetting = false)
        {
            IsDeleted = true;
            AddDomainEvent(new DeletedShiftEvent(this, isGeneralSetting));
        }
        #endregion
    }
    public class AddShift
    {
        public string Name { get; set; }
        public long From { get; set; }
        public long To { get; set; }
        public bool IsActive { get; set; }
        public int BranchId { get; set; }
        public long CheckInBefore { get; set; }
        public long CheckOutAfter { get; set; }
        public bool IsGeneralSetting { get; set; }
    }

}
