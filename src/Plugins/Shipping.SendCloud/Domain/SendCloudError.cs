using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Domain
{
    public class SendCloudError
    {
        public Error error { get; set; }
    }
    public class Error
    {
        public int code { get; set; }
        public string request { get; set; }
        public string message { get; set; }
    }
}
