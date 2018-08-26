using System.Collections.Generic;
using System.Linq;
using TheOne.Redis.Client.Internal;
using TheOne.Redis.Common;

namespace TheOne.Redis.Client {

    public partial class RedisTypedClient<T> {

        public IHasNamed<IRedisSortedSet<T>> SortedSets { get; set; }

        public void AddItemToSortedSet(IRedisSortedSet<T> toSet, T value) {
            this._client.AddItemToSortedSet(toSet.Id, value.ToJson());
        }

        public void AddItemToSortedSet(IRedisSortedSet<T> toSet, T value, double score) {
            this._client.AddItemToSortedSet(toSet.Id, value.ToJson(), score);
        }

        public bool RemoveItemFromSortedSet(IRedisSortedSet<T> fromSet, T value) {
            return this._client.RemoveItemFromSortedSet(fromSet.Id, value.ToJson());
        }

        public T PopItemWithLowestScoreFromSortedSet(IRedisSortedSet<T> fromSet) {
            return DeserializeFromString(this._client.PopItemWithLowestScoreFromSortedSet(fromSet.Id));
        }

        public T PopItemWithHighestScoreFromSortedSet(IRedisSortedSet<T> fromSet) {
            return DeserializeFromString(this._client.PopItemWithHighestScoreFromSortedSet(fromSet.Id));
        }

        public bool SortedSetContainsItem(IRedisSortedSet<T> set, T value) {
            return this._client.SortedSetContainsItem(set.Id, value.ToJson());
        }

        public double IncrementItemInSortedSet(IRedisSortedSet<T> set, T value, double incrementBy) {
            return this._client.IncrementItemInSortedSet(set.Id, value.ToJson(), incrementBy);
        }

        public long GetItemIndexInSortedSet(IRedisSortedSet<T> set, T value) {
            return this._client.GetItemIndexInSortedSet(set.Id, value.ToJson());
        }

        public long GetItemIndexInSortedSetDesc(IRedisSortedSet<T> set, T value) {
            return this._client.GetItemIndexInSortedSetDesc(set.Id, value.ToJson());
        }

        public List<T> GetAllItemsFromSortedSet(IRedisSortedSet<T> set) {
            List<string> list = this._client.GetAllItemsFromSortedSet(set.Id);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetAllItemsFromSortedSetDesc(IRedisSortedSet<T> set) {
            List<string> list = this._client.GetAllItemsFromSortedSetDesc(set.Id);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSet(IRedisSortedSet<T> set, int fromRank, int toRank) {
            List<string> list = this._client.GetRangeFromSortedSet(set.Id, fromRank, toRank);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSetDesc(IRedisSortedSet<T> set, int fromRank, int toRank) {
            List<string> list = this._client.GetRangeFromSortedSetDesc(set.Id, fromRank, toRank);
            return ConvertEachTo<T>(list);
        }

        public IDictionary<T, double> GetAllWithScoresFromSortedSet(IRedisSortedSet<T> set) {
            IDictionary<string, double> map = this._client.GetRangeWithScoresFromSortedSet(set.Id, _firstElement, _lastElement);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSet(IRedisSortedSet<T> set, int fromRank, int toRank) {
            IDictionary<string, double> map = this._client.GetRangeWithScoresFromSortedSet(set.Id, fromRank, toRank);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetDesc(IRedisSortedSet<T> set, int fromRank, int toRank) {
            IDictionary<string, double> map = this._client.GetRangeWithScoresFromSortedSetDesc(set.Id, fromRank, toRank);
            return CreateGenericMap(map);
        }

        public List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore) {
            List<string> list = this._client.GetRangeFromSortedSetByLowestScore(set.Id, fromStringScore, toStringScore);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore, int? skip,
            int? take) {
            List<string> list = this._client.GetRangeFromSortedSetByLowestScore(set.Id, fromStringScore, toStringScore, skip, take);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore) {
            List<string> list = this._client.GetRangeFromSortedSetByLowestScore(set.Id, fromScore, toScore);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore, int? skip, int? take) {
            List<string> list = this._client.GetRangeFromSortedSetByLowestScore(set.Id, fromScore, toScore, skip, take);
            return ConvertEachTo<T>(list);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore,
            string toStringScore) {
            IDictionary<string, double> map =
                this._client.GetRangeWithScoresFromSortedSetByLowestScore(set.Id, fromStringScore, toStringScore);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, string fromStringScore,
            string toStringScore, int? skip, int? take) {
            IDictionary<string, double> map =
                this._client.GetRangeWithScoresFromSortedSetByLowestScore(set.Id, fromStringScore, toStringScore, skip, take);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double>
            GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore) {
            IDictionary<string, double> map = this._client.GetRangeWithScoresFromSortedSetByLowestScore(set.Id, fromScore, toScore);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetByLowestScore(IRedisSortedSet<T> set, double fromScore, double toScore,
            int? skip, int? take) {
            IDictionary<string, double> map =
                this._client.GetRangeWithScoresFromSortedSetByLowestScore(set.Id, fromScore, toScore, skip, take);
            return CreateGenericMap(map);
        }

        public List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore) {
            List<string> list = this._client.GetRangeFromSortedSetByHighestScore(set.Id, fromStringScore, toStringScore);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore, string toStringScore, int? skip,
            int? take) {
            List<string> list = this._client.GetRangeFromSortedSetByHighestScore(set.Id, fromStringScore, toStringScore, skip, take);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore, double toScore) {
            List<string> list = this._client.GetRangeFromSortedSetByHighestScore(set.Id, fromScore, toScore);
            return ConvertEachTo<T>(list);
        }

        public List<T> GetRangeFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore, double toScore, int? skip, int? take) {
            List<string> list = this._client.GetRangeFromSortedSetByHighestScore(set.Id, fromScore, toScore, skip, take);
            return ConvertEachTo<T>(list);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore,
            string toStringScore) {
            IDictionary<string, double> map =
                this._client.GetRangeWithScoresFromSortedSetByHighestScore(set.Id, fromStringScore, toStringScore);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, string fromStringScore,
            string toStringScore, int? skip, int? take) {
            IDictionary<string, double> map =
                this._client.GetRangeWithScoresFromSortedSetByHighestScore(set.Id, fromStringScore, toStringScore, skip, take);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore,
            double toScore) {
            IDictionary<string, double> map = this._client.GetRangeWithScoresFromSortedSetByHighestScore(set.Id, fromScore, toScore);
            return CreateGenericMap(map);
        }

        public IDictionary<T, double> GetRangeWithScoresFromSortedSetByHighestScore(IRedisSortedSet<T> set, double fromScore,
            double toScore, int? skip, int? take) {
            IDictionary<string, double> map =
                this._client.GetRangeWithScoresFromSortedSetByHighestScore(set.Id, fromScore, toScore, skip, take);
            return CreateGenericMap(map);
        }

        public long RemoveRangeFromSortedSet(IRedisSortedSet<T> set, int minRank, int maxRank) {
            return this._client.RemoveRangeFromSortedSet(set.Id, minRank, maxRank);
        }

        public long RemoveRangeFromSortedSetByScore(IRedisSortedSet<T> set, double fromScore, double toScore) {
            return this._client.RemoveRangeFromSortedSetByScore(set.Id, fromScore, toScore);
        }

        public long GetSortedSetCount(IRedisSortedSet<T> set) {
            return this._client.GetSortedSetCount(set.Id);
        }

        public double GetItemScoreInSortedSet(IRedisSortedSet<T> set, T value) {
            return this._client.GetItemScoreInSortedSet(set.Id, value.ToJson());
        }

        public long StoreIntersectFromSortedSets(IRedisSortedSet<T> intoSetId, params IRedisSortedSet<T>[] setIds) {
            return this._client.StoreIntersectFromSortedSets(intoSetId.Id, setIds.Select(x => x.Id).ToArray());
        }

        public long StoreIntersectFromSortedSets(IRedisSortedSet<T> intoSetId, IRedisSortedSet<T>[] setIds, string[] args) {
            return this._client.StoreIntersectFromSortedSets(intoSetId.Id, setIds.Select(x => x.Id).ToArray(), args);
        }

        public long StoreUnionFromSortedSets(IRedisSortedSet<T> intoSetId, params IRedisSortedSet<T>[] setIds) {
            return this._client.StoreUnionFromSortedSets(intoSetId.Id, setIds.Select(x => x.Id).ToArray());
        }

        public long StoreUnionFromSortedSets(IRedisSortedSet<T> intoSetId, IRedisSortedSet<T>[] setIds, string[] args) {
            return this._client.StoreUnionFromSortedSets(intoSetId.Id, setIds.Select(x => x.Id).ToArray(), args);
        }

        private static T DeserializeFromString(string serializedObj) {
            return serializedObj.FromJson<T>();
        }

        private static IDictionary<T, double> CreateGenericMap(IDictionary<string, double> map) {
            var genericMap = new OrderedDictionary<T, double>();
            foreach (KeyValuePair<string, double> entry in map) {
                genericMap[DeserializeFromString(entry.Key)] = entry.Value;
            }

            return genericMap;
        }

        internal class RedisClientSortedSets : IHasNamed<IRedisSortedSet<T>> {

            private readonly RedisTypedClient<T> _client;

            public RedisClientSortedSets(RedisTypedClient<T> client) {
                this._client = client;
            }

            public IRedisSortedSet<T> this[string setId] {
                get => new RedisClientSortedSet<T>(this._client, setId);
                set {
                    IRedisSortedSet<T> col = this[setId];
                    col.Clear();
                    col.CopyTo(value.ToArray(), 0);
                }
            }

        }

    }

}
