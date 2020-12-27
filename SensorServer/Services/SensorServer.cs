﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensorServerApi
{
    public class SensorServer : ISensorServer
    {
        public event OnSensorStatus OnSensorStatusEvent;
        private ConcurrentBag<Sensor> _sensors;
        private readonly int _sensorCount = 3; //200;
        private int _maxDelayBetweenStatusChange = 2000;
        private readonly int _maxDelayForGetSensor = 200;
        private Random _random = new Random();
        private CancellationTokenSource _cancellationTokenSource;

        public SensorServer()
        {
            _sensors = new ConcurrentBag<Sensor>();
            for (int i = 0; i < _sensorCount; i++)
            {
                _sensors.Add(new Sensor()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Sensor {i+1}",
                    SensorType = (SensorType)_random.Next(Enum.GetNames(typeof(SensorType)).Length)
                });
            }
        }

        private Task<SensorStatus> _createRandomSensorStatus()
        {
            SensorStatus sensorStatus = new SensorStatus();
            sensorStatus.StatusType = (StatusType)_random.Next(Enum.GetNames(typeof(StatusType)).Length);
            sensorStatus.IsAlarmStatus = _isAlarmStatus(sensorStatus.StatusType);
            sensorStatus.SensorId = _sensors.ElementAt(_random.Next(_sensorCount)).Id;
            return Task.FromResult(sensorStatus);
        }

        private Task _runCycle()
        {
            return Task.Factory.StartNew(async () => {
                var status = await _createRandomSensorStatus();
                OnSensorStatusEvent?.BeginInvoke(status, null, null);                
            });
        }

        private bool _isAlarmStatus(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.Disconnected:
                case StatusType.Alarm:
                default:
                    return true;
                case StatusType.Connected:
                case StatusType.Default:
                case StatusType.On:
                case StatusType.Off:
                    return false;
            }
        }

        public Task StartServer(Rate rate = Rate.Easy)
        {
            _maxDelayBetweenStatusChange = (int)rate;
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async() => {
                do
                {
                    await _runCycle();
                    await Task.Delay(_random.Next(_maxDelayBetweenStatusChange));
                } while (!_cancellationTokenSource.IsCancellationRequested);
            });
            return Task.CompletedTask;
        }

        public Task StopServer()
        {
            _cancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }

        public async Task<Sensor> GetSensorById(Guid id)
        {
            await Task.Delay(_random.Next(_maxDelayForGetSensor));           
            return _sensors.FirstOrDefault(sensor => sensor.Id == id);
        }
    }
}
