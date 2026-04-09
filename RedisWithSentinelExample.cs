using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Net;

namespace RedisBenchmark
{
    public class RedisWithSentinelExample
    {
        public static async Task RunExampleAsync()
        {
            // Simple connection string with sentinel and serviceName
            var connectionString = "172.17.6.45:26379,serviceName=mymaster"; // Update as needed
            var redis = await ConnectionMultiplexer.ConnectAsync(connectionString);
            var db = redis.GetDatabase();

            // Write a key
            string key = "sentinel:test";
            string value = "hello from sentinel!";
            await db.StringSetAsync(key, value);
            Console.WriteLine($"Wrote key: {key} value: {value}");

            // Read the key
            var readValue = await db.StringGetAsync(key);
            Console.WriteLine($"Read key: {key} value: {readValue}");
        }
    }
}
