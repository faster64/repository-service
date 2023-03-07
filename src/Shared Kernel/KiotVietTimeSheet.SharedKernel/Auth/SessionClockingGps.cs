using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KiotVietTimeSheet.SharedKernel.Auth
{
    public class SessionClockingGps
    {
        [DataMember(Name = "lbcg")]
        public bool LoginByClockingGps { get; set; }

        [DataMember(Name = "perms")]
        public List<string> Permissions { get; set; }
    }
}
