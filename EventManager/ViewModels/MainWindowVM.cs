using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EventManger.ViewModels
{
	public class MainWindowVM : ViewModelBase
	{
		private readonly EventService _eventService;
		private readonly CollectionViewSource _eventsCollection;

		public RelayCommand<Event> RemoveEventCommand { get; private set; }
		public RelayCommand ClearEventsCommand { get; private set; }
		
		private ObservableCollection<Event> _events;
		public ObservableCollection<Event> Events
		{
			get => _events;
			set
			{
				_events = value;
				RaisePropertyChanged();
				App.Current.Dispatcher.Invoke(() => ClearEventsCommand.RaiseCanExecuteChanged());
			}
		}
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
			Events = new ObservableCollection<Event>();
			_eventsCollection = new CollectionViewSource { Source = Events };
			_eventsCollection.SortDescriptions.Add(new SortDescription("TimeRecieved", ListSortDirection.Descending));
			_eventsCollection.IsLiveSortingRequested = true;
			_eventService.EventsCollectionChanged += _eventService_EventsCollectionChanged;
			_eventService.Listen();
		}

		private void _eventService_EventsCollectionChanged(object sender, EventsChangedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() =>
			{
				Events.Clear();
				foreach (var @event in e.Events)
				{
					Events.Add(@event);
				}
				ClearEventsCommand.RaiseCanExecuteChanged();
			});
		}

		private void InitCommands()
		{
			RemoveEventCommand = new RelayCommand<Event>(RemoveEvent);
			ClearEventsCommand = new RelayCommand(ClearEvents, CanClearEvents);
		}

		private bool CanClearEvents()
		{
			return Events?.Count > 3;
		}

		private void ClearEvents()
		{
			_eventService.Reset();
		}

		private void RemoveEvent(Event @event)
		{
			// TODO: Use dependancy injection to inject CacheSerice into view model
			//await _eventService.CacheService.RemoveEntity(SelectedEvent);
			Events.Remove(@event);
			ClearEventsCommand.RaiseCanExecuteChanged();
		}
	}
}
