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
        public bool IsServerLog { get; set; }

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

        public override void OnClose()
        {
            base.OnClose();
            clients.Remove(this);
            UpdateRoom(RoomNumber);

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
                    case "room_request":
                        SendAllRooms();
                        break;

                    case "connection_request":
                        var connection_request = JsonConvert.DeserializeObject<Json.ConnectionRequest>(message);
                        RoomNumber = connection_request.prop_id;

                        if (connection_request.request_type == "connect_to_prop")
                        {
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
                            foreach (var room in Rooms)
                            {
                                if (room.Number == RoomNumber)
                                {
                                    found = true;
                                }
                            }
                            if (!found)
                                Rooms.Add(new HotelRoom(RoomNumber));
                            UpdateRoom(RoomNumber);
                            SendAllValues();
                        }else
                        {
                            if(connection_request.request_type=="server_log")
                            {
                                this.IsServerLog = true;
                            }
                        }
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

                            case "wind_speed":
                                var wind_speed =Convert.ToDouble( set_value.value);
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.WindSpeed = wind_speed;
                                    }
                                }
                                SendValue("wind_speed");
                                break;

                            case "wind_direction":
                                var wind_direction = Convert.ToDouble(set_value.value);
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.WindDirection = wind_direction;
                                    }
                                }
                                SendValue("wind_direction");
                                break;

                            case "userdefined_double_1":
                                var userdefined_double_1 = Convert.ToDouble(set_value.value);
                                foreach(var room in Rooms)
                                {
                                    if(room.Number== RoomNumber)
                                    { room.UserDefinedDouble1 = userdefined_double_1; }
                                }
                                SendValue("userdefined_double_1");
                                break;
                            case "userdefined_double_2":
                                var userdefined_double_2 = Convert.ToDouble(set_value.value);
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    { room.UserDefinedDouble2 = userdefined_double_2; }
                                }
                                SendValue("userdefined_double_2");
                                break;
                            case "userdefined_double_3":
                                var userdefined_double_3 = Convert.ToDouble(set_value.value);
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    { room.UserDefinedDouble3 = userdefined_double_3; }
                                }
                                SendValue("userdefined_double_3");
                                break;
                            case "userdefined_string_1":
                                var userdefined_string_1 = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    { room.UserDefinedString1 = userdefined_string_1; }
                                }
                                SendValue("userdefined_string_1");
                                break;
                            case "userdefined_string_2":
                                var userdefined_string_2 = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    { room.UserDefinedString2 = userdefined_string_2; }
                                }
                                SendValue("userdefined_string_2");
                                break;
                            case "userdefined_string_3":
                                var userdefined_string_3 = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    { room.UserDefinedString3 = userdefined_string_3; }
                                }
                                SendValue("userdefined_string_3");
                                break;


                            case "room4th_temperature":
                                var room4th_temperature = Convert.ToDouble(set_value.value);
                                foreach(var room in Rooms)
                                {
                                    if(room.Number==RoomNumber)
                                    {
                                        room.Room4thTemperature = room4th_temperature;
                                    }
                                }
                                SendValue("room4th_temperature");
                                break;

                            case "room4th_presence_state":
                                var room4th_presence_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room4thPresenceState = room4th_presence_state;
                                    }
                                }
                                SendValue("room4th_presence_state");
                                break;

                            
                            case "room4th_air_state":
                                var room4th_air_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room4thAirState = room4th_air_state;
                                    }
                                }
                                SendValue("room4th_air_state");
                                break;

                            case "room4th_light_state":
                                var room4th_light_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room4thLightState = room4th_light_state;
                                    }
                                }
                                SendValue("room4th_light_state");
                                break;

                            case "room4th_window_state":
                                var room4th_window_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room4thWindowState = room4th_window_state;
                                    }
                                }
                                SendValue("room4th_window_state");
                                break;

                            case "room4th_heater_state":
                                var room4th_heater_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room4thHeaterState = room4th_heater_state;
                                    }
                                }
                                SendValue("room4th_heater_state");
                                break;

                            case "room4th_blind_state":
                                var room4th_blind_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room4thBlindState = room4th_blind_state;
                                    }
                                }
                                SendValue("room4th_blind_state");
                                break;


                            case "room2nd_temperature":
                                var room2nd_temperature = Convert.ToDouble(set_value.value);
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room2ndTemperature = room2nd_temperature;
                                    }
                                }
                                SendValue("room2nd_temperature");
                                break;

                            case "room2nd_presence_state":
                                var room2nd_presence_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room2ndPresenceState = room2nd_presence_state;
                                    }
                                }
                                SendValue("room2nd_presence_state");
                                break;


                            case "room2nd_air_state":
                                var room2nd_air_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room2ndAirState = room2nd_air_state;
                                    }
                                }
                                SendValue("room2nd_air_state");
                                break;

                            case "room2nd_light_state":
                                var room2nd_light_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room2ndLightState = room2nd_light_state;
                                    }
                                }
                                SendValue("room2nd_light_state");
                                break;

                            case "room2nd_window_state":
                                var room2nd_window_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room2ndWindowState = room2nd_window_state;
                                    }
                                }
                                SendValue("room2nd_window_state");
                                break;

                            case "room2nd_heater_state":
                                var room2nd_heater_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room2ndHeaterState = room2nd_heater_state;
                                    }
                                }
                                SendValue("room2nd_heater_state");
                                break;

                            case "room2nd_blind_state":
                                var room2nd_blind_state = set_value.value.ToString();
                                foreach (var room in Rooms)
                                {
                                    if (room.Number == RoomNumber)
                                    {
                                        room.Room2ndBlindState = room2nd_blind_state;
                                    }
                                }
                                SendValue("room2nd_blind_state");
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
        
        private void SendAllRooms()
        {
            foreach(var room in Rooms)
            {
                int clientCounter = 0;
                foreach(MicrosoftWebSockets client in clients)
                {                    
                    if(client.RoomNumber==room.Number)
                    {
                        clientCounter++;
                    }
                }
                var update = new Json.RoomUpdate() { type = "room_update", connected_clients = clientCounter, room_number = room.Number, is_active = clientCounter > 0 };
                Send(JsonConvert.SerializeObject(update));
            }
        }

        private void UpdateRoom(int number)
        {
            foreach (var room in Rooms)
            {
                if (room.Number == number)
                {
                    int clientCounter = 0;
                    foreach (MicrosoftWebSockets client in clients)
                    {
                        if (client.RoomNumber == room.Number)
                        {
                            clientCounter++;
                        }
                    }
                    var update = new Json.RoomUpdate() { type = "room_update", connected_clients = clientCounter, room_number = room.Number, is_active = clientCounter > 0 };

                    foreach (MicrosoftWebSockets client in clients)
                    {
                        if(client.IsServerLog)
                        {
                            client.Send(JsonConvert.SerializeObject(update));
                        }
                    }
                    return;
                }
            }
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

                case "sun_state":
                    var sunState = new Json.SetValue() { type = "property_update", value_name = "sun_state", value = GetRoom().SunState };
                    json = JsonConvert.SerializeObject(sunState);
                    SendJsonToAllClients(json);
                    break;

                case "wind_speed":
                    var windSpeed = new Json.SetValue() { type = "property_update", value_name = "wind_speed", value = GetRoom().WindSpeed };
                    json = JsonConvert.SerializeObject(windSpeed);
                    SendJsonToAllClients(json);
                    break;

                case "wind_direction":
                    var windDirection = new Json.SetValue() { type = "property_update", value_name = "wind_direction", value = GetRoom().WindDirection };
                    json = JsonConvert.SerializeObject(windDirection);
                    SendJsonToAllClients(json);
                    break;

                case "userdefined_double_1":
                    var userdefined_double_1 = new Json.SetValue() { type = "property_update", value_name = "userdefined_double_1", value = GetRoom().UserDefinedDouble1 };
                    json = JsonConvert.SerializeObject(userdefined_double_1);
                    SendJsonToAllClients(json);
                    break;
                case "userdefined_double_2":
                    var userdefined_double_2 = new Json.SetValue() { type = "property_update", value_name = "userdefined_double_2", value = GetRoom().UserDefinedDouble2 };
                    json = JsonConvert.SerializeObject(userdefined_double_2);
                    SendJsonToAllClients(json);
                    break;
                case "userdefined_double_3":
                    var userdefined_double_3 = new Json.SetValue() { type = "property_update", value_name = "userdefined_double_3", value = GetRoom().UserDefinedDouble3 };
                    json = JsonConvert.SerializeObject(userdefined_double_3);
                    SendJsonToAllClients(json);
                    break;
                case "userdefined_string_1":
                    var userdefined_string_1 = new Json.SetValue() { type = "property_update", value_name = "userdefined_string_1", value = GetRoom().UserDefinedString1 };
                    json = JsonConvert.SerializeObject(userdefined_string_1);
                    SendJsonToAllClients(json);
                    break;
                case "userdefined_string_2":
                    var userdefined_string_2 = new Json.SetValue() { type = "property_update", value_name = "userdefined_string_2", value = GetRoom().UserDefinedString2 };
                    json = JsonConvert.SerializeObject(userdefined_string_2);
                    SendJsonToAllClients(json);
                    break;
                case "userdefined_string_3":
                    var userdefined_string_3 = new Json.SetValue() { type = "property_update", value_name = "userdefined_string_3", value = GetRoom().UserDefinedString3 };
                    json = JsonConvert.SerializeObject(userdefined_string_3);
                    SendJsonToAllClients(json);
                    break;


                case "room4th_temperature":
                    var room4thTemp = new Json.SetValue() { type = "property_update", value_name = "room4th_temperature", value = GetRoom().Room4thTemperature };
                    json = JsonConvert.SerializeObject(room4thTemp);
                    SendJsonToAllClients(json);
                    break;

                case "room4th_presence_state":
                    var room4thPresenceState = new Json.SetValue() { type = "property_update", value_name = "room4th_presence_state", value = GetRoom().Room4thPresenceState};
                    json = JsonConvert.SerializeObject(room4thPresenceState);
                    SendJsonToAllClients(json);
                    break;

                case "room4th_air_state":
                    var room4thAirState = new Json.SetValue() { type = "property_update", value_name = "room4th_air_state", value = GetRoom().Room4thAirState };
                    json = JsonConvert.SerializeObject(room4thAirState);
                    SendJsonToAllClients(json);
                    break;
                    
                case "room4th_light_state":
                    var room4thLightState = new Json.SetValue() { type = "property_update", value_name = "room4th_light_state", value = GetRoom().Room4thLightState };
                    json = JsonConvert.SerializeObject(room4thLightState);
                    SendJsonToAllClients(json);
                    break;

                case "room4th_window_state":
                    var room4thWindowState = new Json.SetValue() { type = "property_update", value_name = "room4th_window_state", value = GetRoom().Room4thWindowState };
                    json = JsonConvert.SerializeObject(room4thWindowState);
                    SendJsonToAllClients(json);
                    break;

                case "room4th_heater_state":
                    var room4thHeaterState = new Json.SetValue() { type = "property_update", value_name = "room4th_heater_state", value = GetRoom().Room4thHeaterState };
                    json = JsonConvert.SerializeObject(room4thHeaterState);
                    SendJsonToAllClients(json);
                    break;

                case "room4th_blind_state":
                    var room4thBlindState = new Json.SetValue() { type = "property_update", value_name = "room4th_blind_state", value = GetRoom().Room4thBlindState };
                    json = JsonConvert.SerializeObject(room4thBlindState);
                    SendJsonToAllClients(json);
                    break;


                case "room2nd_temperature":
                    var room2ndTemp = new Json.SetValue() { type = "property_update", value_name = "room2nd_temperature", value = GetRoom().Room2ndTemperature };
                    json = JsonConvert.SerializeObject(room2ndTemp);
                    SendJsonToAllClients(json);
                    break;

                case "room2nd_presence_state":
                    var room2ndPresenceState = new Json.SetValue() { type = "property_update", value_name = "room2nd_presence_state", value = GetRoom().Room2ndPresenceState };
                    json = JsonConvert.SerializeObject(room2ndPresenceState);
                    SendJsonToAllClients(json);
                    break;

                case "room2nd_air_state":
                    var room2ndAirState = new Json.SetValue() { type = "property_update", value_name = "room2nd_air_state", value = GetRoom().Room2ndAirState };
                    json = JsonConvert.SerializeObject(room2ndAirState);
                    SendJsonToAllClients(json);
                    break;

                case "room2nd_light_state":
                    var room2ndLightState = new Json.SetValue() { type = "property_update", value_name = "room2nd_light_state", value = GetRoom().Room2ndLightState };
                    json = JsonConvert.SerializeObject(room2ndLightState);
                    SendJsonToAllClients(json);
                    break;

                case "room2nd_window_state":
                    var room2ndWindowState = new Json.SetValue() { type = "property_update", value_name = "room2nd_window_state", value = GetRoom().Room2ndWindowState };
                    json = JsonConvert.SerializeObject(room2ndWindowState);
                    SendJsonToAllClients(json);
                    break;

                case "room2nd_heater_state":
                    var room2ndHeaterState = new Json.SetValue() { type = "property_update", value_name = "room2nd_heater_state", value = GetRoom().Room2ndHeaterState };
                    json = JsonConvert.SerializeObject(room2ndHeaterState);
                    SendJsonToAllClients(json);
                    break;

                case "room2nd_blind_state":
                    var room2ndBlindState = new Json.SetValue() { type = "property_update", value_name = "room2nd_blind_state", value = GetRoom().Room2ndBlindState };
                    json = JsonConvert.SerializeObject(room2ndBlindState);
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
                var roomnumberupdate = new Json.SetValue() { type = "property_update", value_name = "prop_number", value = RoomNumber };
                Send(JsonConvert.SerializeObject(roomnumberupdate));
                var ambientTemp = new Json.SetValue() { type = "property_update", value_name = "ambient_temperature", value = room.AmbientTemperature };
                Send(JsonConvert.SerializeObject(ambientTemp));
                var sunState = new Json.SetValue() { type = "property_update", value_name = "sun_state", value = room.SunState };
                Send(JsonConvert.SerializeObject(sunState));
                var windSpeed = new Json.SetValue() { type = "property_update", value_name = "wind_speed", value = GetRoom().WindSpeed };
                Send(JsonConvert.SerializeObject(windSpeed));
                var windDirection = new Json.SetValue() { type = "property_update", value_name = "wind_direction", value = GetRoom().WindDirection };
                Send(JsonConvert.SerializeObject(windDirection));

                var userdefined_double_1 = new Json.SetValue() { type = "property_update", value_name = "userdefined_double_1", value = GetRoom().UserDefinedDouble1};
                Send(JsonConvert.SerializeObject(userdefined_double_1));
                var userdefined_double_2 = new Json.SetValue() { type = "property_update", value_name = "userdefined_double_2", value = GetRoom().UserDefinedDouble2 };
                Send(JsonConvert.SerializeObject(userdefined_double_2));
                var userdefined_double_3 = new Json.SetValue() { type = "property_update", value_name = "userdefined_double_3", value = GetRoom().UserDefinedDouble3 };
                Send(JsonConvert.SerializeObject(userdefined_double_3));
                var userdefined_string_1 = new Json.SetValue() { type = "property_update", value_name = "userdefined_string_1", value = GetRoom().UserDefinedString1};
                Send(JsonConvert.SerializeObject(userdefined_string_1));
                var userdefined_string_2 = new Json.SetValue() { type = "property_update", value_name = "userdefined_string_2", value = GetRoom().UserDefinedString2 };
                Send(JsonConvert.SerializeObject(userdefined_string_2));
                var userdefined_string_3 = new Json.SetValue() { type = "property_update", value_name = "userdefined_string_3", value = GetRoom().UserDefinedString3 };
                Send(JsonConvert.SerializeObject(userdefined_string_3));

                var room4thTemp = new Json.SetValue() { type = "property_update", value_name = "room4th_temperature", value = GetRoom().Room4thTemperature };
                Send(JsonConvert.SerializeObject(room4thTemp));
                var room4thPresenceState = new Json.SetValue() { type = "property_update", value_name = "room4th_presence_state", value = GetRoom().Room4thPresenceState };
                Send(JsonConvert.SerializeObject(room4thPresenceState));
                var room4thAirState = new Json.SetValue() { type = "property_update", value_name = "room4th_air_state", value = GetRoom().Room4thAirState };
                Send(JsonConvert.SerializeObject(room4thAirState));
                var room4thLightState = new Json.SetValue() { type = "property_update", value_name = "room4th_light_state", value = GetRoom().Room4thLightState };
                Send(JsonConvert.SerializeObject(room4thLightState));
                var room4thWindowState = new Json.SetValue() { type = "property_update", value_name = "room4th_window_state", value = GetRoom().Room4thWindowState };
                Send(JsonConvert.SerializeObject(room4thWindowState));
                var room4thHeaterState = new Json.SetValue() { type = "property_update", value_name = "room4th_heater_state", value = GetRoom().Room4thHeaterState };
                Send(JsonConvert.SerializeObject(room4thHeaterState));
                var room4thBlindState = new Json.SetValue() { type = "property_update", value_name = "room4th_blind_state", value = GetRoom().Room4thBlindState };
                Send(JsonConvert.SerializeObject(room4thBlindState));

                var room2ndTemp = new Json.SetValue() { type = "property_update", value_name = "room2nd_temperature", value = GetRoom().Room2ndTemperature };
                Send(JsonConvert.SerializeObject(room2ndTemp));
                var room2ndPresenceState = new Json.SetValue() { type = "property_update", value_name = "room2nd_presence_state", value = GetRoom().Room2ndPresenceState };
                Send(JsonConvert.SerializeObject(room2ndPresenceState));
                var room2ndAirState = new Json.SetValue() { type = "property_update", value_name = "room2nd_air_state", value = GetRoom().Room2ndAirState };
                Send(JsonConvert.SerializeObject(room2ndAirState));
                var room2ndLightState = new Json.SetValue() { type = "property_update", value_name = "room2nd_light_state", value = GetRoom().Room2ndLightState };
                Send(JsonConvert.SerializeObject(room2ndLightState));
                var room2ndWindowState = new Json.SetValue() { type = "property_update", value_name = "room2nd_window_state", value = GetRoom().Room2ndWindowState };
                Send(JsonConvert.SerializeObject(room2ndWindowState));
                var room2ndHeaterState = new Json.SetValue() { type = "property_update", value_name = "room2nd_heater_state", value = GetRoom().Room2ndHeaterState };
                Send(JsonConvert.SerializeObject(room2ndHeaterState));
                var room2ndBlindState = new Json.SetValue() { type = "property_update", value_name = "room2nd_blind_state", value = GetRoom().Room2ndBlindState };
                Send(JsonConvert.SerializeObject(room2ndBlindState));
            }
        }
    }
}