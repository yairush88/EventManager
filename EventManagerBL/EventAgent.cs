using SensorServerApi;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventManagerBL
{
	public class EventAgent
	{
		private ISensorServer _sensorServer;
		private ICacheService _cacheService;
		private const int ClearOldEventsInterval = 2000;
		private Timer _timer;
		private static readonly object syncObject = new object();

		public event EventHandler<Event> OnSensorEvent;

		public List<Event> Events { get; set; }

		public EventAgent()
		{
			_sensorServer = new SensorServer();
			_cacheService = new CacheService();
			Events = new List<Event>();
		}

		public void Listen()
		{
			//Events = GetEventMocks();
			_sensorServer.StartServer(Rate.Easy);
			_sensorServer.OnSensorStatusEvent += _sensorServer_OnSensorStatusEvent;

			//StartEventsTimer();
		}

		public void StopListening()
		{
			_sensorServer.OnSensorStatusEvent -= _sensorServer_OnSensorStatusEvent;
			_timer.Dispose();
		}

		private void OnEvent(Event e)
		{
			OnSensorEvent?.BeginInvoke(this, e, null, null);
		}

		private void StartEventsTimer()
		{
			_timer = new Timer(RemoveOldEvents, null, 0, ClearOldEventsInterval);
		}

		private async void RemoveOldEvents(object state)
		{
			var oldEvents = Events.Where(e => (e.TimeRecieved - DateTime.Now).TotalSeconds > 30);

			foreach (var oldEvent in oldEvents)
			{
				await _cacheService.RemoveEntity(oldEvent);
				Events.Remove(oldEvent);
			}
		}

		private async void _sensorServer_OnSensorStatusEvent(SensorStatus sensorStatus)
		{
			var e = await CreateEvent(sensorStatus);
			OnEvent(e);
		}

		private async Task<Event> CreateEvent(SensorStatus sensorStatus)
		{
			var sensor = await _sensorServer.GetSensorById(sensorStatus.SensorId);
			var isAlarming = IsAlarming(sensorStatus, sensor);
			var existingEvent = Events.FirstOrDefault(e => e.Id == sensor.Id);
			Event newEvent = null;

			if (isAlarming)
			{
				if (existingEvent != null)
				{
					// Event exists for this sensor
					existingEvent.Alarms.Add(CreateEventAlarm(existingEvent));
					await _cacheService.UpdateEntity(existingEvent);
				}
				else
				{
					// Sensor doesn't have an event so create one
					newEvent = CreateNewEvent(sensorStatus, sensor);
					newEvent.Alarms.Add(CreateEventAlarm(newEvent));
					await _cacheService.AddEntity(newEvent);

					Events = GetEventMocks();
					//Events.Add(newEvent);
				}
			}
			else if (existingEvent != null)
			{
				// Event exists for this sensor
				existingEvent.Alarms.Add(CreateEventAlarm(existingEvent));
				await _cacheService.UpdateEntity(existingEvent);
			}

			return existingEvent ?? newEvent;
		}

		private EventAlarm CreateEventAlarm(Event sensorEvent)
		{
			return new EventAlarm
			{
				TimeRecieved = sensorEvent.TimeRecieved,
				StatusType = sensorEvent.StatusType,
				IsAlarming = sensorEvent.IsAlarming
			};
		}

		private Event CreateNewEvent(SensorStatus sensorStatus, Sensor sensor)
		{
			var sensorEvent = new Event
			{
				Id = sensor.Id,
				Name = sensor.Name,
				TimeRecieved = sensorStatus.TimeStamp,
				IsAlarming = IsAlarming(sensorStatus, sensor),
				StatusType = sensorStatus.StatusType.ToString()
			};

			return sensorEvent;
		}

		private bool IsAlarming(SensorStatus sensorStatus, Sensor sensor)
		{
			bool isAlarming;
			switch (sensor.SensorType)
			{
				case SensorType.Video:
					isAlarming = sensorStatus.StatusType == StatusType.Disconnected || sensorStatus.StatusType == StatusType.Alarm;
					break;
				case SensorType.Fence:
					isAlarming = sensorStatus.StatusType == StatusType.Disconnected || sensorStatus.StatusType == StatusType.Alarm ||
						sensorStatus.StatusType == StatusType.Off;
					break;
				case SensorType.AccessControl:
					isAlarming = sensorStatus.StatusType == StatusType.Disconnected || sensorStatus.StatusType == StatusType.Alarm ||
						sensorStatus.StatusType == StatusType.On;
					break;
				case SensorType.FireDetection:
					isAlarming = sensorStatus.StatusType != StatusType.Default;
					break;
				case SensorType.Radar:
					isAlarming = sensorStatus.StatusType == StatusType.Alarm || sensorStatus.StatusType == StatusType.Off;
					break;
				default:
					isAlarming = sensorStatus.IsAlarmStatus;
					break;
			}

			return isAlarming;
		}

		public List<Event> GetEventMocks()
		{
			var events = new List<Event>()
			{
				new Event {Name = "Sensor 1", InitialStatusType = "Connected", StatusType = "Disconnected",  TimeRecieved = DateTime.Now, IsAlarming = true },
				new Event {Name = "Sensor 2", InitialStatusType = "Disconnected", StatusType = "Alram", TimeRecieved = DateTime.Now, IsAlarming = false },
				new Event {Name = "Sensor 3", InitialStatusType = "Connected", StatusType = "Connected", TimeRecieved = DateTime.Now, IsAlarming = false },
				new Event {Name = "Sensor 4", InitialStatusType = "Alarm", StatusType = "Disconnected", TimeRecieved = DateTime.Now, IsAlarming = true },
				new Event {Name = "Sensor 5", InitialStatusType = "On", StatusType = "Alram", TimeRecieved = DateTime.Now, IsAlarming = true },
				new Event {Name = "Sensor 6", InitialStatusType = "Connected", StatusType = "Connected", TimeRecieved = DateTime.Now, IsAlarming = false }
			};

			events[0].Alarms = new List<EventAlarm>
			{
				new EventAlarm {StatusType = "Connected", IsAlarming=false, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Off", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Disconnected", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=false, TimeRecieved=DateTime.Now}
			};

			events[1].Alarms = new List<EventAlarm>
			{
				new EventAlarm {StatusType = "Off", IsAlarming=false, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=true, TimeRecieved=DateTime.Now},
			};

			events[2].Alarms = new List<EventAlarm>
			{
				new EventAlarm {StatusType = "Disconnected", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Off", IsAlarming=false, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=true, TimeRecieved=DateTime.Now},

			};

			events[3].Alarms = new List<EventAlarm>
			{
				new EventAlarm {StatusType = "On", IsAlarming=false, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Alarm", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Off", IsAlarming=true, TimeRecieved=DateTime.Now},

			};

			return events;
		}
	}
}
