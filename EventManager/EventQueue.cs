//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EventManger
//{
//	public class EventQueue : IDisposable
//	{
//		BlockingCollection<Event> _taskQ = new BlockingCollection<Event>();

//		public EventQueue(int workerCount)
//		{
//			for (int i = 0; i < workerCount; i++)
//			{
//				Task.Factory.StartNew(Consume);
//			}
//		}

//		public void Enqueue(Event @event)
//		{
//			_taskQ.Add(@event);
//		}
		
//		private Event Consume()
//		{
//			foreach (var @event in _taskQ.GetConsumingEnumerable())
//			{
//				yield
//			}
//		}

//		public void Dispose()
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
