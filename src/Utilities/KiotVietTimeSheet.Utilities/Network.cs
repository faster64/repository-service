using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace KiotVietTimeSheet.Utilities
{
    public static class Network
    {
        public static bool IsIpAddressValid(string currentIp, string lstaddress)
        {
            var incomingOctets = currentIp.Trim().Split('.');
            if (string.IsNullOrEmpty(currentIp) || string.IsNullOrEmpty(lstaddress))
                return false;
            var validIpAddresses = lstaddress.Trim().Split(';');
            foreach (var validIpAddress in validIpAddresses)
            {
                if (validIpAddress.Trim() == currentIp)
                    return true;

                //Split the valid IP address into it's 4 octets 
                var validOctets = validIpAddress.Trim().Split('.');

                var matches = !validOctets.Where((t, index) => t != "*" && t != incomingOctets[index]).Any();

                //Iterate through each octet 

                if (matches)
                    return true;
            }
            //Found no matches 
            return false;
        }

    }
}
