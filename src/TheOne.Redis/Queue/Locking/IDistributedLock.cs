using TheOne.Redis.Client;

namespace TheOne.Redis.Queue.Locking {

    /// <summary>
    ///     Distributed lock interface
    /// </summary>
    public interface IDistributedLock {

        long Lock(string key, int acquisitionTimeout, int lockTimeout, out long lockExpire, IRedisClient client);
        bool Unlock(string key, long lockExpire, IRedisClient client);

    }

}
