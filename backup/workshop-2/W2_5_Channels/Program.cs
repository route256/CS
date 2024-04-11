using System.Net.Http.Json;
using System.Threading.Channels;

namespace W2_5_Channels;

public class Program
{
    public static async Task Main()
    {
        var channel = Channel.CreateBounded<Name>(30);
        
        Task producer1 = Task.Run(async () => { await NameProducer.Produce(channel.Writer); });
        Task consumer1 = Task.Run(async () => { await NameConsumer.Enrich(channel.Reader); });
        Task consumer2 = Task.Run(async () => { await NameConsumer.Enrich(channel.Reader); });
        Task consumer3 = Task.Run(async () => { await NameConsumer.Enrich(channel.Reader); });
        Task consumer4 = Task.Run(async () => { await NameConsumer.Enrich(channel.Reader); });
        Task consumer5 = Task.Run(async () => { await NameConsumer.Enrich(channel.Reader); });
        
        await Task.WhenAll(producer1, consumer1, consumer2, consumer3, consumer4, consumer5);
    }
    
    public static class NameProducer
    {
        public static async Task Produce(ChannelWriter<Name> channelWriter)
        {
            await foreach (var name in Repository.GetAll())
            {
                await channelWriter.WriteAsync(name);
            }
            
            channelWriter.Complete();
        }
    }

    public static class NameConsumer
    {
        public static async Task Enrich(ChannelReader<Name> channelReader)
        {
            try
            {
                while (true)
                {
                    var item = await channelReader.ReadAsync();
                    
                    Console.WriteLine($"{item.first}, {item.last}, {item.title}");
                    
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                
                //wait foreach (var name in channelReader.ReadAllAsync())
                //
                //   Console.WriteLine($"{name.first}, {name.last}, {name.title}");
                //   
                //   await Task.Delay(TimeSpan.FromSeconds(1));
                //

            }
            catch (ChannelClosedException)
            {
                Console.WriteLine("Channel was closed");
            }
        }
    }
    
    public static class Repository
    {
        public static async IAsyncEnumerable<Name> GetAll()
        {
            using var httpClient = new HttpClient();

            for (int i = 0; i < 10; i++)
            {
                var root = await httpClient.GetFromJsonAsync<Root>("https://randomuser.me/api?inc=name&results=10");

                foreach (var result in root.results)
                {
                    yield return result.name;
                }
            }
        }
    }
    
    public class Info
    {
        public string seed { get; set; }
        public int results { get; set; }
        public int page { get; set; }
        public string version { get; set; }
    }

    public class Name
    {
        public string title { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }

    public class Result
    {
        public Name name { get; set; }
    }

    public class Root
    {
        public List<Result> results { get; set; }
        public Info info { get; set; }
    }
}