using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchuecoEEHACK.Json
{
    public class RoomUpdate : AbstractJson
    {
        public int room_number { get; set; }
        public int connected_clients { get; set; }
        public bool is_active { get; set; }
    }
}