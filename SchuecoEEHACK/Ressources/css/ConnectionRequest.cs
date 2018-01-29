using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EEHack.Json
{
    public class ConnectionRequest:AbstractJson
    {
        public string request_type { get; set; }
        public int room_number { get; set; }
    }
}