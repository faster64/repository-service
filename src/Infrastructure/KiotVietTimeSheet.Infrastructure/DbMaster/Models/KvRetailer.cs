using System;
using System.Security.AccessControl;

namespace KiotVietTimeSheet.Infrastructure.DbMaster.Models
{
    public class KvRetailer
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string LocationName { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string WardName { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Code { get; set; }

        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }

        public bool IsAdminActive { get; set; }

        public int GroupId { get; set; }

        public int? IndustryId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? TimeSheetBlockUnit { get; set; }
        public int? ContractType { get; set; }

        public bool IsFnB()
        {
            return IndustryId == 15;
        }        
        
        public bool IsBooking()
        {
            return IndustryId == 16;
        }
    }
}
