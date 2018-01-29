
using Microsoft.ServiceModel.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchuecoEEHACK
{
    public class HotelRoom
    {
        public int Number { get; private set; }
        public double AmbientTemperature { get; set; }
        public string SunState { get; set; }
        public string AirState { get; set; }
        public HotelRoom(int number)
        {
            Number = number;
            AmbientTemperature = 15;
            SunState = "Cloudy";
            AirState = "Used Air";
        }
    }
}