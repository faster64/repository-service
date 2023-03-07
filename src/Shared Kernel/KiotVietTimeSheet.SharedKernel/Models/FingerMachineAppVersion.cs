using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.SharedKernel.Models
{
    [DataContract]
    public class FingerMachineAppVersion
    {
        [DataMember(Name = "kv-ketnoi-app")]
        public Item Item { get; set; } 
    }
    [DataContract]    
    public class Item
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }
        [DataMember(Name = "groupid")]
        public List<int> GroupIds { get; set; }
    }
}