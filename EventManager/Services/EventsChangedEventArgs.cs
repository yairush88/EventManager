using System.Collections.Generic;

namespace EventManger
{
	public class EventsChangedEventArgs
	{
		public List<Event> Events { get; set; }

		public EventsChangedEventArgs(List<Event> events)
		{
			Events = events;
		}
	}
}
