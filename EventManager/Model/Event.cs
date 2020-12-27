using SensorServerApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EventManger
{
	public class Event : IIdEntity, INotifyPropertyChanged
	{
		private string _statusType;
		private DateTime _timeRecieved;

		public Guid Id { get; set; }
		public string Name { get; set; }
		public string StatusType 
		{
			get => _statusType;
			set
			{
				_statusType = value;
				OnPropertyChanged();
			}
		}
		public string InitialStatusType { get; set; }
		public bool IsAlarming { get; set; }
		public DateTime TimeRecieved 
		{
			get => _timeRecieved;
			set
			{
				_timeRecieved = value;
				OnPropertyChanged();
			}
		}
		public ObservableCollection<EventAlarm> Alarms { get; set; } = new ObservableCollection<EventAlarm>();

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
