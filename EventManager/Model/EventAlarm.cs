﻿using System;

namespace EventManger
{
	public class EventAlarm
	{
		public DateTime TimeRecieved { get; set; }
		public string StatusType { get; set; }
		public bool IsAlarming { get; set; }
	}
}
