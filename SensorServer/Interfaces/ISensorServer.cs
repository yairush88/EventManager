using System;
using System.Threading.Tasks;

namespace SensorServerApi
{
	public delegate void OnSensorStatus(SensorStatus sensorStatus);

    public interface ISensorServer
    {
        event OnSensorStatus OnSensorStatusEvent;
        Task StartServer(Rate rate = Rate.Easy);
        Task StopServer();
        Task<Sensor> GetSensorById(Guid id);
    }
}
