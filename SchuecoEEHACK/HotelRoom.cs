
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
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }

        public double UserDefinedDouble1 { get; set; }
        public double UserDefinedDouble2 { get; set; }
        public double UserDefinedDouble3 { get; set; }
        public string UserDefinedString1 { get; set; }
        public string UserDefinedString2 { get; set; }
        public string UserDefinedString3 { get; set; }


        public double Room4thTemperature { get; set; }
        public string Room4thPresenceState { get; set; }
        public string Room4thAirState { get; set; }
        public string Room4thLightState { get; set;}
        public string Room4thWindowState { get; set; }
        public string Room4thHeaterState { get; set; }
        public string Room4thBlindState { get; set; }

        public double Room2ndTemperature { get; set; }
        public string Room2ndPresenceState { get; set; }
        public string Room2ndAirState { get; set; }
        public string Room2ndLightState { get; set; }
        public string Room2ndWindowState { get; set; }
        public string Room2ndHeaterState { get; set; }
        public string Room2ndBlindState { get; set; }
        public HotelRoom(int number)
        {
            Number = number;
            AmbientTemperature = 15;
            SunState = "Cloudy";
            WindSpeed = 5;
            WindDirection = 270;

            UserDefinedDouble1 = 0;
            UserDefinedDouble2 = 0;
            UserDefinedDouble3 = 0;
            UserDefinedString1 = "none ";
            UserDefinedString2 = "none 2";
            UserDefinedString3 = "none 3";

            Room4thTemperature = 18;
            Room4thPresenceState = "Unused";
            Room4thAirState = "Used Air";
            Room4thLightState = "Off";
            Room4thWindowState = "Closed";
            Room4thHeaterState = "On";
            Room4thBlindState = "Open";

            Room2ndTemperature = 20;
            Room2ndPresenceState = "Used";
            Room2ndAirState = "Good Air";
            Room2ndLightState = "On";
            Room2ndWindowState = "Tilt";
            Room2ndHeaterState = "Off";
            Room2ndBlindState = "Closed";
        }
    }
}