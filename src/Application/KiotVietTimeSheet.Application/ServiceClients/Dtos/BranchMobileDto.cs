namespace KiotVietTimeSheet.Application.ServiceClients.Dtos
{
    public class BranchMobileDto : BranchDto
    {
        public string Address { get; set; }
        public string WardName { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public bool IsActive { get; set; }
        public bool LimitAccess { get; set; }
    }
}
