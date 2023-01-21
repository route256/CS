using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks.Dataflow;

namespace W2_6_Dataflow;

public class Program
{
    public static async Task Main()
    {
        var linkOptions = new DataflowLinkOptions()
        {
            PropagateCompletion = true,
        };

        var writeRawMessagesBlock = new TransformManyBlock<JsonArray, string>(usersJson =>
        {
            return usersJson.Select(x => x.ToJsonString());
        });

        var deserializeUsersBlock = new TransformBlock<string, Result>(userJson => JsonSerializer.Deserialize<Result>(userJson));
        var mapBlock = new TransformBlock<Result, IdentifiedUser>(r => new IdentifiedUser(){Id = Guid.NewGuid(), Login = $"{r.name.first} {r.name.last} {r.name.title}", ImageUri = r.picture.medium});
        var processingBlock = new BroadcastBlock<IdentifiedUser>(i => i);
        var downloadImageBlock = new TransformBlock<IdentifiedUser, UserPicture>(async item =>
        {
            using var httpClient = new HttpClient();
            var imgBytes = await httpClient.GetByteArrayAsync(item.ImageUri);

            return new UserPicture()
            {
                Id = item.Id,
                Login = item.Login,
                Bytes = imgBytes,
            };
        });

        var saveUserImgBlock = new ActionBlock<UserPicture>(item => File.WriteAllBytes($"results/{item.Id}.jpg", item.Bytes));
        var saveUserJsonBlock = new ActionBlock<IdentifiedUser>(item =>
        {
            var json = JsonSerializer.Serialize(item, new JsonSerializerOptions() {WriteIndented = true});
            File.WriteAllText($"results/{item.Id}.json", json);
        });

        writeRawMessagesBlock.LinkTo(deserializeUsersBlock, linkOptions);
        deserializeUsersBlock.LinkTo(mapBlock, linkOptions);
        mapBlock.LinkTo(processingBlock, linkOptions);

        processingBlock.LinkTo(downloadImageBlock, linkOptions);
        processingBlock.LinkTo(saveUserJsonBlock, linkOptions);

        downloadImageBlock.LinkTo(saveUserImgBlock, linkOptions);

        var mainTask = UsersProducer.Start(writeRawMessagesBlock);

        await Task.WhenAll(mainTask, saveUserJsonBlock.Completion, saveUserImgBlock.Completion);
    }

    public static class UsersProducer
    {
        public static async Task Start(TransformManyBlock<JsonArray, string> targetBlock)
        {
            await foreach (var userJson in Repository.GetAll())
            {
                await targetBlock.SendAsync(userJson);
            }
            
            targetBlock.Complete();
        }
    }
    
    
    public static class Repository
    {
        public static async IAsyncEnumerable<JsonArray> GetAll()
        {
            using var httpClient = new HttpClient();

            for (int i = 0; i < 10; i++)
            {
                var json = await httpClient.GetStringAsync("https://randomuser.me/api?inc=name,picture&results=10");

                var jObject = JsonNode.Parse(json).AsObject(); 
                
                yield return jObject["results"].AsArray();
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

    public class Picture
    {
        public string large { get; set; }
        public string medium { get; set; }
        public string thumbnail { get; set; }
    }

    public class Result
    {
        public Name name { get; set; }
        public Picture picture { get; set; }
    }

    public class Root
    {
        public List<Result> results { get; set; }
        public Info info { get; set; }
    }

    public class IdentifiedUser
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string ImageUri { get; set; }
    }

    public class UserPicture
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public byte[] Bytes { get; set; }
    }
}