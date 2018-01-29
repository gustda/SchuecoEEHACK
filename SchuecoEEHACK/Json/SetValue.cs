using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchuecoEEHACK.Json
{
    public class SetValue:AbstractJson
    {
        public string value_name { get; set; }
        public object value { get; set; }
    }
}