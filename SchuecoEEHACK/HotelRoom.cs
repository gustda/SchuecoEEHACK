
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
        public double RoomTemperature { get; set; }
        public string PresenceState { get; set; }
        public string SunState { get; set; }
        public string AirState { get; set; }
        public string LightState { get; set;}
        public string WindowState { get; set; }
        public string HeaterState { get; set; }
        public string BlindState { get; set; }
        public HotelRoom(int number)
        {
            Number = number;
            AmbientTemperature = 15;
            RoomTemperature = 18;
            PresenceState = "Unused";
            SunState = "Cloudy";
            AirState = "Used Air";
            LightState = "Off";
            WindowState = "Closed";
            HeaterState = "On";
            BlindState = "Open";
        }
    }
}