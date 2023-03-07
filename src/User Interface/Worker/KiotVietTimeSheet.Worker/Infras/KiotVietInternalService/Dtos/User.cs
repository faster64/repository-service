namespace KiotVietTimeSheet.AuditTrailWorker.Infras.KiotVietInternalService.Dtos
{
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
