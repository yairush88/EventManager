using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace EventManger.ViewModels
{
	public class MainWindowVM : ViewModelBase
	{
		private EventService _eventService;

		public RelayCommand RemoveEventCommand { get; private set; }
		public ObservableCollection<Event> Events => _eventService.Events;

		private Event _selectedEvent;
		public Event SelectedEvent
		{
			get => _selectedEvent;
			set
			{
				_selectedEvent = value;
				RaisePropertyChanged();
			}
		}

		public MainWindowVM()
		{
			InitCommands();
			_eventService = new EventService();
			_eventService.Listen();

			// For testing UI
			//SelectedEvent = Events[1];
		}

		private void InitCommands()
		{
			RemoveEventCommand = new RelayCommand(this.RemoveEvent, true);
		}

		private void RemoveEvent()
		{
			Events.Remove(SelectedEvent);
		}
	}
}
