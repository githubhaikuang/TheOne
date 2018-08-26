using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheOne.Redis.Client.Internal;
using TheOne.Redis.Common;
using TheOne.Redis.Pipeline;

namespace TheOne.Redis.Client {

    /// <summary>
    ///     Allows you to get Redis value operations to operate against POCO types.
    /// </summary>
    public partial class RedisTypedClient<T> : IRedisTypedClient<T> {

        private readonly RedisClient _client;
        private readonly string _recentSortedSetKey;

        static RedisTypedClient() {
            Client.RedisClient.UniqueTypes.Add(typeof(T));
        }

        /// <summary>
        ///     Use this to share the same redis connection with another
        /// </summary>
        /// <param name="client" >The client.</param>
        public RedisTypedClient(RedisClient client) {
            this._client = client;
            this.Lists = new RedisClientLists(this);
            this.Sets = new RedisClientSets(this);
            this.SortedSets = new RedisClientSortedSets(this);

            this.SequenceKey = client.GetTypeSequenceKey<T>();
            this.TypeIdsSetKey = client.GetTypeIdsSetKey<T>();
            this.TypeLockKey = string.Concat(client.NamespacePrefix, "lock:", typeof(T).Name);
            this._recentSortedSetKey = string.Concat(client.NamespacePrefix, "recent:", typeof(T).Name);
        }

        public IRedisNativeClient NativeClient => this._client;

        public string TypeIdsSetKey { get; set; }
        public string TypeLockKey { get; set; }

        public IRedisTransactionBase Transaction {
            get => this._client.Transaction;
            set => this._client.Transaction = value;
        }

        public IRedisPipelineShared Pipeline {
            get => this._client.Pipeline;
            set => this._client.Pipeline = value;
        }

        public IRedisClient RedisClient => this._client;

        public IRedisTypedTransaction<T> CreateTransaction() {
            return new RedisTypedTransaction<T>(this);
        }

        public IRedisTypedPipeline<T> CreatePipeline() {
            return new RedisTypedPipeline<T>(this);
        }

        public IDisposable AcquireLock() {
            return this._client.AcquireLock(this.TypeLockKey);
        }

        public IDisposable AcquireLock(TimeSpan timeout) {
            return this._client.AcquireLock(this.TypeLockKey, timeout);
        }

        public List<string> GetAllKeys() {
            return this._client.GetAllKeys();
        }

        public string UrnKey(T entity) {
            return this._client.UrnKey(entity);
        }

        public IRedisSet TypeIdsSet => new RedisClientSet(this._client, this._client.GetTypeIdsSetKey<T>());

        public T this[string key] {
            get => this.GetValue(key);
            set => this.SetValue(key, value);
        }

        public void SetValue(string key, T entity) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            this._client.Set(key, this.SerializeValue(entity));
            this._client.RegisterTypeId(entity);
        }

        public void SetValue(string key, T entity, TimeSpan expireIn) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            this._client.Set(key, this.SerializeValue(entity), expireIn);
            this._client.RegisterTypeId(entity);
        }

        public bool SetValueIfNotExists(string key, T entity) {
            var success = this._client.SetNX(key, this.SerializeValue(entity)) == RedisNativeClient.Success;
            if (success) {
                this._client.RegisterTypeId(entity);
            }

            return success;
        }

        public bool SetValueIfExists(string key, T entity) {
            var success = this._client.Set(key, this.SerializeValue(entity), true);
            if (success) {
                this._client.RegisterTypeId(entity);
            }

            return success;
        }

        public T GetValue(string key) {
            return this.DeserializeValue(this._client.Get(key));
        }

        public T GetAndSetValue(string key, T value) {
            return this.DeserializeValue(this._client.GetSet(key, this.SerializeValue(value)));
        }

        public bool ContainsKey(string key) {
            return this._client.Exists(key) == RedisNativeClient.Success;
        }

        public bool RemoveEntry(string key) {
            return this._client.Del(key) == RedisNativeClient.Success;
        }

        public bool RemoveEntry(params string[] keys) {
            return this._client.Del(keys) == RedisNativeClient.Success;
        }

        public bool RemoveEntry(params IHasRedisStringId[] entities) {
            List<string> ids = entities.Select(x => x.Id).ToList();
            var success = this._client.Del(ids.ToArray()) == RedisNativeClient.Success;
            if (success) {
                this._client.RemoveTypeIds(ids.ToArray());
            }

            return success;
        }

        public long IncrementValue(string key) {
            return this._client.Incr(key);
        }

        public long IncrementValueBy(string key, int count) {
            return this._client.IncrBy(key, count);
        }

        public long DecrementValue(string key) {
            return this._client.Decr(key);
        }

        public long DecrementValueBy(string key, int count) {
            return this._client.DecrBy(key, count);
        }

        public string SequenceKey { get; set; }

        public void SetSequence(int value) {
            this._client.GetSet(this.SequenceKey, Encoding.UTF8.GetBytes(value.ToString()));
        }

        public long GetNextSequence() {
            return this.IncrementValue(this.SequenceKey);
        }

        public long GetNextSequence(int incrBy) {
            return this.IncrementValueBy(this.SequenceKey, incrBy);
        }

        public RedisKeyType GetEntryType(string key) {
            return this._client.GetEntryType(key);
        }

        public string GetRandomKey() {
            return this._client.RandomKey();
        }

        public bool ExpireEntryIn(string key, TimeSpan expireIn) {
            return this._client.ExpireEntryIn(key, expireIn);
        }

        public bool ExpireEntryAt(string key, DateTime expireAt) {
            return this._client.ExpireEntryAt(key, expireAt);
        }

        public bool ExpireIn(object id, TimeSpan expireIn) {
            var key = this._client.UrnKey<T>(id);
            return this._client.ExpireEntryIn(key, expireIn);
        }

        public bool ExpireAt(object id, DateTime expireAt) {
            var key = this._client.UrnKey<T>(id);
            return this._client.ExpireEntryAt(key, expireAt);
        }

        public TimeSpan GetTimeToLive(string key) {
            return TimeSpan.FromSeconds(this._client.Ttl(key));
        }

        public void Save() {
            this._client.Save();
        }

        public void SaveAsync() {
            this._client.SaveAsync();
        }

        public void FlushDb() {
            this._client.FlushDb();
        }

        public void FlushAll() {
            this._client.FlushAll();
        }

        public T[] SearchKeys(string pattern) {
            List<string> strKeys = this._client.SearchKeys(pattern);
            var keysCount = strKeys.Count;

            var keys = new T[keysCount];
            for (var i = 0; i < keysCount; i++) {
                keys[i] = strKeys[i].FromJson<T>();
            }

            return keys;
        }

        public List<T> GetValues(List<string> keys) {
            if (keys == null || keys.Count == 0) {
                return new List<T>();
            }

            byte[][] resultBytesArray = this._client.MGet(keys.ToArray());

            var results = new List<T>();
            foreach (byte[] resultBytes in resultBytesArray) {
                if (resultBytes == null) {
                    continue;
                }

                T result = this.DeserializeValue(resultBytes);
                results.Add(result);
            }

            return results;
        }

        public void StoreAsHash(T entity) {
            this._client.StoreAsHash(entity);
        }

        public T GetFromHash(object id) {
            return this._client.GetFromHash<T>(id);
        }

        public void Watch(params string[] keys) {
            this._client.Watch(keys);
        }

        public void UnWatch() {
            this._client.UnWatch();
        }

        public void Multi() {
            this._client.Multi();
        }

        public void Discard() {
            this._client.Discard();
        }

        public void Exec() {
            this._client.Exec();
        }

        internal void AddTypeIdsRegisteredDuringPipeline() {
            this._client.AddTypeIdsRegisteredDuringPipeline();
        }

        internal void ClearTypeIdsRegisteredDuringPipeline() {
            this._client.ClearTypeIdsRegisteredDuringPipeline();
        }

        public byte[] SerializeValue(T value) {
            var strValue = value.ToJson();
            return Encoding.UTF8.GetBytes(strValue);
        }

        public T DeserializeValue(byte[] value) {
            var strValue = value != null ? Encoding.UTF8.GetString(value) : null;
            return strValue.FromJson<T>();
        }

        internal void ExpectQueued() {
            this._client.ExpectQueued();
        }

        internal void ExpectOk() {
            this._client.ExpectOk();
        }

        internal int ReadMultiDataResultCount() {
            return this._client.ReadMultiDataResultCount();
        }

        public void FlushSendBuffer() {
            this._client.FlushAndResetSendBuffer();
        }

        public void ResetSendBuffer() {
            this._client.ResetSendBuffer();
        }

        internal void EndPipeline() {
            this._client.EndPipeline();
        }

    }

}
