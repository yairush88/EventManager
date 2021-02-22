using System;

namespace SensorServerApi
{
	public class EventAlarm : IIdEntity
    {
        public Guid Id { get; set; }
        public DateTime TimeRecieved { get; set; }
        public Guid SensorId { get; set; }
        public StatusType StatusType { get; set; }

        public EventAlarm(SensorStatus sensorStatus)
        {
            Id = sensorStatus.Id;
            TimeRecieved = sensorStatus.TimeStamp;
            SensorId = sensorStatus.SensorId;
            StatusType = sensorStatus.StatusType;
        }
    }
}
