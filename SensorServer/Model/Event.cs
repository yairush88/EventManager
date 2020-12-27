using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorServerApi
{
    public class Event : IIdEntity
    {
        public Guid Id { get; set; }
        public Guid CreatedSensorId { get; set; }
        public DateTime CreationTime { get; set; }
        public List<EventAlarm> EventAlarms { get; set; }

        public Event() {}

        public Event(SensorStatus sensorStatus)
        {
            Id = Guid.NewGuid();
            CreatedSensorId = sensorStatus.SensorId;
            CreationTime = DateTime.Now;
            EventAlarms = new List<EventAlarm>() { new EventAlarm(sensorStatus) };
        }
    }
}
