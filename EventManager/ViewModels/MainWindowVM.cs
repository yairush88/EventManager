using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EventManger.ViewModels
{
	public class MainWindowVM : ViewModelBase
	{
		private EventService _eventService;
		private CollectionViewSource _eventsCollection;

		public RelayCommand<Event> RemoveEventCommand { get; private set; }
		public ObservableCollection<Event> Events => _eventService.Events;
		public ICollectionView EventsCollection => _eventsCollection.View;


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
			_eventsCollection = new CollectionViewSource { Source = Events };
			_eventsCollection.SortDescriptions.Add(new SortDescription("TimeRecieved", ListSortDirection.Descending));
			_eventsCollection.IsLiveSortingRequested = true;

			_eventService.Listen();
		}

		private void InitCommands()
		{
			RemoveEventCommand = new RelayCommand<Event>(RemoveEvent);
		}

		private void RemoveEvent(Event @event)
		{
			// TODO: Use dependancy injection to inject CacheSerice into view model
			//await _eventService.CacheService.RemoveEntity(SelectedEvent);
			Events.Remove(@event);
		}
	}
}
