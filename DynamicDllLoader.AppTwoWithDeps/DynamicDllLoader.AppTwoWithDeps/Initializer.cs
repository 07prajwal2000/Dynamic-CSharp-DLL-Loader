using RestSharp;
using StackExchange.Redis;

namespace DynamicDllLoader.AppTwoWithDeps;

public class Initializer : IInitializer
{
    public void Start()
    {
        Console.WriteLine("App two with deps started updated");
        try
        {
            var options = new RestClientOptions("https://jsonplaceholder.typicode.com");
            using var client = new RestClient(options);
            var todos = client.Get<Todo[]>("/todos");
            Console.WriteLine("Todos count: " + todos.Length);

            using var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var db = redis.GetDatabase(0);
            var val = db.StringGet("key1");
            if (val.HasValue)
            {
                Console.WriteLine("Contains value in redis: " + val);
            }
            db.StringSet("key1", "value1");
            db.KeyExpire("key1", TimeSpan.FromSeconds(20));
            Console.WriteLine("Redis value set successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    public void Stop()
    {
        Console.WriteLine("App two with deps stopped");
    }
}

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool Completed { get; set; }
}