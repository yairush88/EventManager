using System.Threading.Tasks;

namespace SensorServerApi
{
	public interface ICacheService
    {
        Task<bool> AddEntity<T>(T ent) where T : class, IIdEntity, new();
        Task<bool> UpdateEntity<T>(T ent) where T : class, IIdEntity, new();
        Task<bool> RemoveEntity<T>(T ent) where T : class, IIdEntity, new();
    }
}
