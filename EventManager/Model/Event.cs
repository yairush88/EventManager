﻿using SensorServerApi;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventManger
{
	public class Event : IIdEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string StatusType { get; set; }
		public string InitialStatusType { get; set; }
		public bool IsAlarming { get; set; }
		public DateTime TimeRecieved { get; set; }
		public List<EventAlarm> Alarms { get; set; } = new List<EventAlarm>();
	}
}