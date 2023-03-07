using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Extension;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models
{
    public class Holiday : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete,
        ICacheable
    {
        #region properties
        public long Id { get; set; }

        /// <summary>
        /// Tên kì nghỉ lễ tết
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Ngày bắt đầu kì nghỉ lễ tết
        /// </summary>
        public DateTime From { get; protected set; }

        /// <summary>
        /// Ngày kết thúc kì nghỉ lễ tết
        /// </summary>
        public DateTime To { get; protected set; }

        /// <summary>
        /// Id của cửa hàng/ doanh nghiệp
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Id người tạo kì nghỉ lễ tết
        /// </summary>
        public long CreatedBy { get; set; }

        /// <summary>
        /// Thời điểm tạo kì nghỉ lễ tết
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Id người thay đổi thông tin kì nghỉ lễ tết
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Thời điểm thay đổi thông tin kì nghỉ lễ tết
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Trạng thái của kì nghỉ lễ tết
        /// true - kì nghỉ lễ tết đã xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Thời điểm xóa kì nghỉ lễ tết
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Id người xóa kì nghỉ lễ tết
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Số ngày nghỉ trong kì nghỉ lễ tết
        /// </summary>
        public int Days { get; protected set; }

        #endregion

        #region Constructor

        public Holiday()
        {
            IsDeleted = false;
        }

        [JsonConstructor]
        public Holiday(long id, string name, DateTime from, DateTime to)
        {
            Id = id;
            Name = name;
            From = from;
            To = to;
        }

        // Only copy primitive values
        public Holiday(Holiday holiday)
        {
            Id = holiday.Id;
            Name = holiday.Name;
            From = holiday.From;
            To = holiday.To;
            TenantId = holiday.TenantId;
            CreatedBy = holiday.CreatedBy;
            CreatedDate = holiday.CreatedDate;
            ModifiedBy = holiday.ModifiedBy;
            ModifiedDate = holiday.ModifiedDate;
            IsDeleted = holiday.IsDeleted;
            DeletedDate = holiday.DeletedDate;
            DeletedBy = holiday.DeletedBy;

            if (holiday.DomainEvents != null)
            {
                foreach (var domainEvent in holiday.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        /// <summary>
        /// Tạo mới kỳ nghỉ / lễ tết từ các tham số truyền vào 
        /// </summary>
        /// <param name="name">Tên kỳ nghỉ / lễ tết</param>
        /// <param name="from">Ngày bắt đầu</param>
        /// <param name="to">Ngày kết thúc</param>
        /// <returns></returns>
        public Holiday(string name, DateTime from, DateTime to)
        {
            Name = (name ?? string.Empty).ToPerfectString();
            From = GetFromDate(from);
            To = GetToDate(to);

            AddDomainEvent(new CreatedHolidayEvent(this));
        }
        #endregion

        #region public method
        /// <summary>
        /// Cập nhật kỳ nghỉ / lễ tết từ các tham số truyền vào
        /// </summary>
        /// <param name="name"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public void Update(string name, DateTime from, DateTime to)
        {
            var beforeChange = MemberwiseClone() as Holiday;

            Name = (name ?? string.Empty).Trim();
            From = GetFromDate(from);
            To = GetToDate(to);

            AddDomainEvent(new UpdatedHolidayEvent(beforeChange, this));
        }

        /// <summary>
        /// Xóa kì nghỉ lễ tết.
        /// </summary>
        public void Delete()
        {
            IsDeleted = true;
            AddDomainEvent(new DeletedHolidayEvent(this));
        }

        /// <summary>
        /// Kiểm tra có phải là ngày nghỉ lễ hay không?
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool IsDayOfHoliday(DateTime day)
        {
            return From.Date <= day.Date && To.Date >= day.Date;
        }

        public bool IsEqual(Holiday holiday)
        {
            return holiday != null &&
                   From == holiday.From &&
                   To == holiday.To;
        }
        #endregion

        #region private method
        private static DateTime GetFromDate(DateTime from)
        {
            if ( from == default(DateTime))
            {
                return default(DateTime);
            }

            return from.Date;
        }

        private static DateTime GetToDate(DateTime to)
        {
            if (to == default(DateTime))
            {
                return default(DateTime);
            }

            return new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
        }

        #endregion
    }
}
