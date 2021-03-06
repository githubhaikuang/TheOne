using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using TheOne.Redis.Client;
using TheOne.Redis.ClientManager;
using TheOne.Redis.Common;
using TheOne.Redis.Sentinel;

namespace TheOne.Redis.Tests.Sentinel {

    [TestFixture]
    internal sealed class RedisSentinelTests : RedisTestBase {

        private RedisClient _redisSentinel;

        [SetUp]
        public void OnBeforeEachTest() {
            this._redisSentinel = new RedisClient(Config.Sentinel26380);
        }

        [TearDown]
        public void OnAfterEachTest() {
            this._redisSentinel.Dispose();
        }

        [Test]
        public void Can_Get_Master_Addr() {
            List<string> addr = this._redisSentinel.SentinelGetMasterAddrByName(Config.SentinelMasterName);

            var host = addr[0];
            var port = addr[1];
            var hostString = string.Format("{0}:{1}", host, port);

            Assert.That(hostString, Is.EqualTo(Config.Sentinel6380));
        }

        [Test]
        public void Can_Get_Redis_ClientsManager() {
            using (var sentinel = new RedisSentinel(Config.SentinelHosts, Config.SentinelMasterName)) {
                IRedisClientManager clientsManager = sentinel.Start();
                using (IRedisClient client = clientsManager.GetClient()) {
                    Assert.That(client.GetHostString(), Is.EqualTo(Config.Sentinel6380));
                }
            }
        }

        [Test]
        public void Can_Get_Sentinel_Master() {
            Dictionary<string, string> master = this._redisSentinel.SentinelMaster(Config.SentinelMasterName);
            Console.WriteLine(master.ToJson());

            var host = string.Format("{0}:{1}", master["ip"], master["port"]);
            Assert.That(master["name"], Is.EqualTo(Config.SentinelMasterName));
            Assert.That(host, Is.EqualTo(Config.Sentinel6380));
        }

        [Test]
        public void Can_Get_Sentinel_Masters() {
            List<Dictionary<string, string>> masters = this._redisSentinel.SentinelMasters();
            Console.WriteLine(masters.ToJson());

            Assert.That(masters.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Can_Get_Sentinel_Sentinels() {
            List<Dictionary<string, string>> sentinels = this._redisSentinel.SentinelSentinels(Config.SentinelMasterName);
            Console.WriteLine(sentinels.ToJson());

            Assert.That(sentinels.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Can_Get_Sentinel_Slaves() {
            List<Dictionary<string, string>> slaves = this._redisSentinel.SentinelSlaves(Config.SentinelMasterName);
            Console.WriteLine(slaves.ToJson());

            Assert.That(slaves.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Can_Ping_Sentinel() {
            Assert.True(this._redisSentinel.Ping());
        }

        [Test]
        public void Can_specify_db_on_RedisSentinel() {
            using (var sentinel = new RedisSentinel(Config.SentinelHosts, Config.SentinelMasterName)) {
                sentinel.HostFilter = host => string.Format("{0}?db=1", host);

                using (IRedisClientManager clientsManager = sentinel.Start()) {
                    using (IRedisClient client = clientsManager.GetClient()) {
                        Assert.That(client.Db, Is.EqualTo(1));
                    }
                }
            }
        }

        [Test]
        public void Can_specify_Timeout_on_RedisManager() {
            using (var sentinel = new RedisSentinel(Config.SentinelHosts, Config.SentinelMasterName)) {
                sentinel.RedisManagerFactory = (masters, slaves) => new PooledRedisClientManager(masters, slaves) { IdleTimeOutSecs = 20 };

                using (var clientsManager = (PooledRedisClientManager)sentinel.Start()) {
                    using (IRedisClient client = clientsManager.GetClient()) {
                        Assert.That(clientsManager.IdleTimeOutSecs, Is.EqualTo(20));
                        Assert.That(((RedisNativeClient)client).IdleTimeOutSecs, Is.EqualTo(20));
                    }
                }
            }
        }

        [Test]
        public void Defaults_to_default_sentinel_port() {
            RedisEndpoint sentinelEndpoint = RedisEndpoint.Create("127.0.0.1", RedisConfig.DefaultPortSentinel);
            Assert.That(sentinelEndpoint.Port, Is.EqualTo(RedisConfig.DefaultPortSentinel));
        }

        [Test]
        public void Does_scan_for_other_active_sentinels() {
            using (var sentinel = new RedisSentinel(Config.Sentinel26380) {
                ScanForOtherSentinels = true
            }) {
                IRedisClientManager clientsManager = sentinel.Start();

                Assert.That(sentinel.SentinelHosts, Is.EquivalentTo(Config.SentinelHosts));

                using (IRedisClient client = clientsManager.GetClient()) {
                    Assert.That(client.GetHostString(), Is.EqualTo(Config.Sentinel6380));
                }
            }
        }

        [Test]
        [Ignore("Long running test")]
        public void Run_sentinel_for_10_minutes() {
            using (var sentinel = new RedisSentinel(Config.SentinelHosts, Config.SentinelMasterName)) {
                sentinel.OnFailover = manager => {
                    Console.WriteLine("Redis Managers Failed Over to new hosts");
                };
                sentinel.OnWorkerError = ex => {
                    Console.WriteLine("Worker error: {0}", ex);
                };
                sentinel.OnSentinelMessageReceived = (channel, msg) => {
                    Console.WriteLine("Received '{0}' on channel '{1}' from Sentinel", channel, msg);
                };

                using (IRedisClientManager redisManager = sentinel.Start()) {
                    void TimerCallback(object state) {
                        Console.WriteLine("Incrementing key");

                        string key = null;
                        using (IRedisClient redis = redisManager.GetClient()) {
                            var counter = redis.Increment("key", 1);
                            key = "key" + counter;
                            Console.WriteLine("Set key {0} in read/write client", key);
                            redis.SetValue(key, "value" + 1);
                        }

                        using (IRedisClient redis = redisManager.GetClient()) {
                            Console.WriteLine("Get key {0} in read-only client...", key);
                            var value = redis.GetValue(key);
                            Console.WriteLine("{0} = {1}", key, value);
                        }
                    }

                    var aTimer = new Timer(TimerCallback, null, 0, 1000);
                }
            }

            Thread.Sleep(TimeSpan.FromMinutes(10));
        }

    }

}
