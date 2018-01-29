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
            var connectionData = this.WebSocketContext.QueryString["ConnectionData"];         
            connectionData = connectionData.Substring(connectionData.IndexOf('=') + 1);

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

            

            switch (messageTyp)
            {
                case "connection established":                    
                    break;

                case "connect Logo":
                    // es wird ein neues Logo Device erzeugt und hier eine Referenz darauf gespeichert um auf Events reagieren zu können
                    
                    break;

                case "disconnect Logo":                    
                    
                    break;

                case "Send Value AM4":
                    var value = message.Substring(message.IndexOf(':')+1);
                    var number = Convert.ToInt16(value);
                    break;

            }


            
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
                var ambientTemp = new Json.SetValue() { type= "property_update", value_name = "ambient_temperature", value = room.AmbientTemperature };
                Send(JsonConvert.SerializeObject(ambientTemp));
                var sunState = new Json.SetValue() { type = "property_update", value_name = "sun_state", value = room.SunState };
                Send(JsonConvert.SerializeObject(sunState));
                var airState = new Json.SetValue() { type = "property_update", value_name = "air_state", value = room.AirState};
                Send(JsonConvert.SerializeObject(airState));
            }
        }
    }
}