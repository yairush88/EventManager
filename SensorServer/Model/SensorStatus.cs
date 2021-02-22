using System;

namespace SensorServerApi
{
	public class SensorStatus
    {
        public Guid Id { get; set; }
        public bool IsAlarmStatus { get; set; }
        public StatusType StatusType { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid SensorId { get; set; }

        public SensorStatus()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
        }
    }
}
