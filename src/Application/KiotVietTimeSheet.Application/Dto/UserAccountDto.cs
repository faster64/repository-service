using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.Dto
{
    public class UserAccountDto
    {
        public long Id { get; set; }
        public string GivenName { get; set; }
        public string Language { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}