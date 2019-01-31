using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchuecoEEHACK.Json
{
    public class ConnectionRequest:AbstractJson
    {
        public string request_type { get; set; }
        public int prop_id { get; set; }
    }
}