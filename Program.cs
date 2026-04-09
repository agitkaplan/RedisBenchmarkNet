using StackExchange.Redis;
using System.Diagnostics;

namespace RedisBenchmark
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var connectionString = args.Length > 0 ? args[0] : throw new Exception("Enter the connection string.");

                int parallelUsers = 50;
                int operationsPerUser = 1000;
                string keyPrefix = "bench:";

                var redis = await ConnectionMultiplexer.ConnectAsync(connectionString);
                var db = redis.GetDatabase();

                // Find and print the master node
                foreach (var endpoint in redis.GetEndPoints())
                {
                    var server = redis.GetServer(endpoint);
                    if (server.IsConnected && server.ServerType == ServerType.Standalone)
                    {
                        var role = await server.RoleAsync();
                        if (role?.ToString().ToLower().Contains("master") == true)
                        {
                            Console.WriteLine($"Current master node: {endpoint}");
                        }
                    }
                }

                var sw = Stopwatch.StartNew();
                var tasks = new Task[parallelUsers];

                Console.WriteLine($"Benchmark started.");

                for (int i = 0; i < parallelUsers; i++)
                {
                    int userId = i;
                    tasks[i] = Task.Run(async () =>
                    {
                        for (int j = 0; j < operationsPerUser; j++)
                        {
                            string key = $"{keyPrefix}{userId}:{j}";
                            await db.StringSetAsync(key, j);
                            await db.StringGetAsync(key);
                        }
                    });
                }
                await Task.WhenAll(tasks);
                sw.Stop();

                Console.WriteLine($"Benchmark complete: {parallelUsers * operationsPerUser * 2} operations in {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
