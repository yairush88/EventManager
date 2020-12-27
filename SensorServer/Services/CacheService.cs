using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorServerApi
{
    public class CacheService : ICacheService
    {
        private readonly int _maxDelayTime = 5000;
        private Random _random = new Random();

        public async Task<bool> AddEntity<T>(T ent) where T : class, IIdEntity, new()
        {
            // delay the result to mock server slow operations
            await Task.Delay(_random.Next(_maxDelayTime));
            return true;
        }

        public async Task<bool> UpdateEntity<T>(T ent) where T : class, IIdEntity, new()
        {
            // delay the result to mock server slow operations
            await Task.Delay(_random.Next(_maxDelayTime));
            return true;
        }

        public async Task<bool> RemoveEntity<T>(T ent) where T : class, IIdEntity, new()
        {
            // delay the result to mock server slow operations
            await Task.Delay(_random.Next(_maxDelayTime));
            return true;
        }
    }
}
