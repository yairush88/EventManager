using System;

namespace SensorServerApi
{
	public class Sensor : IIdEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SensorType SensorType { get; set; }
    }
}
