using SensorServerApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace EventManger
{
	public class EventService
	{
		private ISensorServer _sensorServer;
		private ICacheService _cacheService;
		private const int ClearOldEventsInterval = 10000;
		private Timer _timer;
		private static readonly object syncObject = new object();

		public ICacheService CacheService => _cacheService;

		public ObservableCollection<Event> Events { get; set; }

		public EventService()
		{
			// TODO: Use IoC Container!
			_sensorServer = new SensorServer();
			_cacheService = new CacheService();
			
			Events = new ObservableCollection<Event>();
			BindingOperations.EnableCollectionSynchronization(Events, syncObject);
		}

		public void Listen()
		{
			// Uncomment for debugging UI
			//Events = GetEventMocks();

			// Comment out the following two lines for testing event mocks instead of real data from server, only for UI debugging
			_sensorServer.StartServer(Rate.Hardcore);
			_sensorServer.OnSensorStatusEvent += _sensorServer_OnSensorStatusEvent;

			StartEventsTimer();
		}

		public void StopListening()
		{
			_sensorServer.OnSensorStatusEvent -= _sensorServer_OnSensorStatusEvent;
			_timer.Dispose();
		}

		private void StartEventsTimer()
		{
			_timer = new Timer(RemoveOldEvents, null, 0, ClearOldEventsInterval);
		}

		private void RemoveOldEvents(object state)
		{
			IEnumerable<Event> oldEvents;
			lock (syncObject)
			{
				oldEvents = Events.Where(e => (DateTime.Now - e.TimeRecieved).TotalSeconds > 30).ToList();

				foreach (var oldEvent in oldEvents)
				{
					_cacheService.RemoveEntity(oldEvent);
					Events.Remove(oldEvent);
				}
			}
		}

		private void _sensorServer_OnSensorStatusEvent(SensorStatus sensorStatus)
		{
			CreateEvent(sensorStatus);
		}

		private void CreateEvent(SensorStatus sensorStatus)
		{
			var sensor = _sensorServer.GetSensorById(sensorStatus.SensorId).Result;
			var isAlarming = IsAlarming(sensorStatus, sensor);

			lock (syncObject)
			{
				var existingEvent = Events.FirstOrDefault(e => e.Id == sensor.Id);

				if (isAlarming)
				{
					if (existingEvent == null)
					{
						// Sensor doesn't have an event so create one
						var newEvent = CreateNewEvent(sensorStatus, sensor);
						newEvent.Alarms.Add(CreateEventAlarm(sensorStatus, isAlarming));
						_cacheService.AddEntity(newEvent);

						Events.Add(newEvent);
					}
					else
					{
						// Event exists for this sensor
						existingEvent.StatusType = sensorStatus.StatusType.ToString();
						existingEvent.TimeRecieved = sensorStatus.TimeStamp;
						existingEvent.IsAlarming = isAlarming;

						// Prevents an exception caused by the current running task when closing the application
						if (App.Current != null)
						{
							App.Current.Dispatcher.Invoke(() => existingEvent.Alarms.Add(CreateEventAlarm(sensorStatus, isAlarming)));
						}
						
						_cacheService.UpdateEntity(existingEvent);
					}
				}
				else if (existingEvent != null)
				{
					// Event exists for this sensor
					existingEvent.StatusType = sensorStatus.StatusType.ToString();
					existingEvent.TimeRecieved = sensorStatus.TimeStamp;
					existingEvent.IsAlarming = isAlarming;

					// Prevents an exception caused by the current running task when closing the application
					if (App.Current != null)
					{
						App.Current.Dispatcher.Invoke(() => existingEvent.Alarms.Add(CreateEventAlarm(sensorStatus, isAlarming)));
					}

					_cacheService.UpdateEntity(existingEvent);
				}
			}
		}

		private EventAlarm CreateEventAlarm(SensorStatus sensorStatus, bool isAlarming)
		{
			return new EventAlarm
			{
				TimeRecieved = sensorStatus.TimeStamp,
				StatusType = sensorStatus.StatusType.ToString(),
				IsAlarming = isAlarming
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
				StatusType = sensorStatus.StatusType.ToString(),
				InitialStatusType = sensorStatus.StatusType.ToString()
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

		public ObservableCollection<Event> GetEventMocks()
		{
			var events = new ObservableCollection<Event>()
			{
				new Event {Name = "Sensor 1", InitialStatusType = "Connected", StatusType = "On",  TimeRecieved = DateTime.Now, IsAlarming = false },
				new Event {Name = "Sensor 2", InitialStatusType = "Off", StatusType = "Alram", TimeRecieved = DateTime.Now, IsAlarming = true },
				new Event {Name = "Sensor 3", InitialStatusType = "Disconnected", StatusType = "Connected", TimeRecieved = DateTime.Now, IsAlarming = false },
				new Event {Name = "Sensor 4", InitialStatusType = "Connected", StatusType = "Disconnected", TimeRecieved = DateTime.Now, IsAlarming = true },
				new Event {Name = "Sensor 5", InitialStatusType = "Off", StatusType = "Alram", TimeRecieved = DateTime.Now, IsAlarming = true },
				new Event {Name = "Sensor 6", InitialStatusType = "On", StatusType = "Connected", TimeRecieved = DateTime.Now, IsAlarming = false }
			};

			events[0].Alarms = new ObservableCollection<EventAlarm>
			{
				new EventAlarm {StatusType = "Connected", IsAlarming=false, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Off", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Disconnected", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=false, TimeRecieved=DateTime.Now}
			};

			events[1].Alarms = new ObservableCollection<EventAlarm>
			{
				new EventAlarm {StatusType = "Off", IsAlarming=false, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=true, TimeRecieved=DateTime.Now},
			};

			events[2].Alarms = new ObservableCollection<EventAlarm>
			{
				new EventAlarm {StatusType = "Disconnected", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=true, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "Off", IsAlarming=false, TimeRecieved=DateTime.Now},
				new EventAlarm {StatusType = "On", IsAlarming=true, TimeRecieved=DateTime.Now},

			};

			events[3].Alarms = new ObservableCollection<EventAlarm>
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
