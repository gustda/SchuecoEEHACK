using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;

namespace SchuecoEEHACK
{
    public class MicrosoftWebSockets:WebSocketHandler
    {
        private static WebSocketCollection clients = new WebSocketCollection();
        private static List<HotelRoom> Rooms;
        public int RoomNumber { get; set; }

        public MicrosoftWebSockets()
        {
            if (null == Rooms)
                Rooms = new List<HotelRoom>();
        }

        public override void OnOpen()
        {
            // wenn eine neue WebSocket verbindung hergestellt wird, wird zuerst die Ziel IP der Steuerung und der Clientname ausgelesen
            // für eine Produktivversion sollte hierzu eine Verbindungsanfrage gesendet werden, dazu ist ein bisschen Protokoll notwendig
            
            // der neu angelegte Client wird in die WebSocket client Liste aufgenommen
            clients.Add(this);            

            // es wird kein Ping benötigt, verbindung bleibt selbstständig aktiv
            //  SetTimer();
        }

        private void Device_NewDataReceived(object sender, EventArgs e)
        {

        }

        
        public override void OnMessage(string message)
        {
            string messageTyp = message;
            //message = message.Replace(@"""", "");
            try
            {
                var json = JsonConvert.DeserializeObject<Json.AbstractJson>(message);

                switch (json.type)
                {
                    case "connection_request":
                        var connection_request = JsonConvert.DeserializeObject<Json.ConnectionRequest>(message);
                        RoomNumber = connection_request.room_number;

                        // a new room is requested
                        if (RoomNumber == 0)
                        {
                            // we need to find a free room
                            for (int i = 100; i < 1000; i++)
                            {
                                bool used = false;
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == i)
                                    {
                                        used = true;
                                        break;
                                    }
                                }
                                // set the unused room number
                                if (!used)
                                {
                                    RoomNumber = i;
                                    break;
                                }
                            }
                        }

                        bool found = false;
                        foreach(var room in Rooms)
                        {
                            if(room.Number== RoomNumber)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                            Rooms.Add(new HotelRoom(RoomNumber));

                        

                        SendAllValues();
                        break;

                    case "set_value":
                        var set_value = JsonConvert.DeserializeObject<Json.SetValue>(message);
                        switch (set_value.value_name)
                        {
                            case "ambient_temperature":
                                var ambient_temperature = Convert.ToDouble(set_value.value);
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.AmbientTemperature = ambient_temperature;
                                    }
                                }
                                SendValue("ambient_temperature");
                                break;

                            case "room_temperature":
                                var room_temperature = Convert.ToDouble(set_value.value);
                                foreach(var room in Rooms)
                                {
                                    if(room.Number==RoomNumber)
                                    {
                                        room.RoomTemperature = room_temperature;
                                    }
                                }
                                SendValue("room_temperature");
                                break;

                            case "presence_state":
                                var presence_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.PresenceState = presence_state;
                                    }
                                }
                                SendValue("presence_state");
                                break;

                            case "sun_state":
                                var sun_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.SunState = sun_state;
                                    }
                                }
                                SendValue("sun_state");
                                break;
                            case "air_state":
                                var air_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.AirState = air_state;
                                    }
                                }
                                SendValue("air_state");
                                break;

                            case "light_state":
                                var light_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.LightState = light_state;
                                    }
                                }
                                SendValue("light_state");
                                break;

                            case "window_state":
                                var window_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.WindowState = window_state;
                                    }
                                }
                                SendValue("window_state");
                                break;

                            case "heater_state":
                                var heater_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.HeaterState = heater_state;
                                    }
                                }
                                SendValue("heater_state");
                                break;

                            case "blind_state":
                                var blind_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.BlindState = blind_state;
                                    }
                                }
                                SendValue("blind_state");
                                break;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                int i = 0;
            }
            
            if(message.Contains(':'))
                messageTyp = message.Substring(0, message.IndexOf(':'));
        }

        private HotelRoom GetRoom()
        {
            foreach (var room in Rooms)
            {
                if (room.Number == RoomNumber)
                {
                    return room;
                }
            }
            return null;
        }
        
        private void SendValue(string valueName)
        {
            string json;
            switch (valueName)
            {
                case "ambient_temperature":
                    var ambientTemp = new Json.SetValue() { type = "property_update", value_name = "ambient_temperature", value = GetRoom().AmbientTemperature };
                     json = JsonConvert.SerializeObject(ambientTemp);
                    SendJsonToAllClients(json);
                    break;

                case "room_temperature":
                    var roomTemp = new Json.SetValue() { type = "property_update", value_name = "room_temperature", value = GetRoom().RoomTemperature };
                    json = JsonConvert.SerializeObject(roomTemp);
                    SendJsonToAllClients(json);
                    break;

                case "presence_state":
                    var presenceState = new Json.SetValue() { type = "property_update", value_name = "presence_state", value = GetRoom().PresenceState};
                    json = JsonConvert.SerializeObject(presenceState);
                    SendJsonToAllClients(json);
                    break;

                case "sun_state":
                    var sunState = new Json.SetValue() { type = "property_update", value_name = "sun_state", value = GetRoom().SunState };
                     json = JsonConvert.SerializeObject(sunState);
                    SendJsonToAllClients(json);
                    break;

                case "air_state":
                    var airState = new Json.SetValue() { type = "property_update", value_name = "air_state", value = GetRoom().AirState };
                    json = JsonConvert.SerializeObject(airState);
                    SendJsonToAllClients(json);
                    break;
                    
                case "light_state":
                    var lightState = new Json.SetValue() { type = "property_update", value_name = "light_state", value = GetRoom().LightState };
                    json = JsonConvert.SerializeObject(lightState);
                    SendJsonToAllClients(json);
                    break;

                case "window_state":
                    var windowState = new Json.SetValue() { type = "property_update", value_name = "window_state", value = GetRoom().WindowState };
                    json = JsonConvert.SerializeObject(windowState);
                    SendJsonToAllClients(json);
                    break;

                case "heater_state":
                    var heaterState = new Json.SetValue() { type = "property_update", value_name = "heater_state", value = GetRoom().HeaterState };
                    json = JsonConvert.SerializeObject(heaterState);
                    SendJsonToAllClients(json);
                    break;

                case "blind_state":
                    var blindState = new Json.SetValue() { type = "property_update", value_name = "blind_state", value = GetRoom().BlindState };
                    json = JsonConvert.SerializeObject(blindState);
                    SendJsonToAllClients(json);
                    break;
                  
            }
        }

        private void SendJsonToAllClients(string json)
        {
            foreach(var client in clients)
            {
                if ((client as MicrosoftWebSockets).RoomNumber==RoomNumber)
                {
                    client.Send(json);
                }
            }
        }

        private void SendAllValues()
        {
            var room = GetRoom();
            if (null != room)
            {
                var roomnumberupdate = new Json.SetValue() { type = "property_update", value_name = "room_number", value = RoomNumber };
                Send(JsonConvert.SerializeObject(roomnumberupdate));
                var ambientTemp = new Json.SetValue() { type= "property_update", value_name = "ambient_temperature", value = room.AmbientTemperature };
                Send(JsonConvert.SerializeObject(ambientTemp));
                var roomTemp = new Json.SetValue() { type = "property_update", value_name = "room_temperature", value = room.RoomTemperature };
                Send(JsonConvert.SerializeObject(roomTemp));
                var presenceState = new Json.SetValue() { type = "property_update", value_name = "presence_state", value = room.PresenceState };
                Send(JsonConvert.SerializeObject(presenceState));
                var sunState = new Json.SetValue() { type = "property_update", value_name = "sun_state", value = room.SunState };
                Send(JsonConvert.SerializeObject(sunState));
                var airState = new Json.SetValue() { type = "property_update", value_name = "air_state", value = room.AirState};
                Send(JsonConvert.SerializeObject(airState));

                var lightState = new Json.SetValue() { type = "property_update", value_name = "light_state", value = room.LightState };
                Send(JsonConvert.SerializeObject(lightState));
                var windowState = new Json.SetValue() { type = "property_update", value_name = "window_state", value = room.WindowState };
                Send(JsonConvert.SerializeObject(windowState));
                var heaterState = new Json.SetValue() { type = "property_update", value_name = "heater_state", value = room.HeaterState };
                Send(JsonConvert.SerializeObject(heaterState));
                var blindState = new Json.SetValue() { type = "property_update", value_name = "blind_state", value = room.BlindState};
                Send(JsonConvert.SerializeObject(blindState));

            }
        }
    }
}