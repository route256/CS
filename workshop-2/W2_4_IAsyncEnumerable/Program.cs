using System.Text.Json;
using System.Web;

namespace W2_4_IAsyncEnumerable;

public class IAsyncEnumerableMain
{
    public static async Task Main()
    {
        var responses = GetEntriesInfoAsync();
            
        await foreach (var info in responses)
        {
            Console.WriteLine("response by '{0}' [{1}]", info.SearchQuery, info.Content);
        }
        
        //var infos = await GetEntriesInfo();
        //
        //foreach (var info in infos)
        //{
        //    Console.WriteLine("response by '{0}' [{1}]", info.SearchQuery, info.Content);
        //}
    }

    public static async Task<IReadOnlyCollection<EntryInfo>> GetEntriesInfo()
    {
        var repository = new Repository();
        var queries = await repository.GetAll();

        var entriesContent = new List<EntryInfo>();
        
        using var httpClient = new HttpClient();
        
        foreach (var query in queries)
        {
            var content = await httpClient.GetStringAsync("https://yandex.ru/search/?text=" + HttpUtility.UrlEncode(query.SearchQuery));

            var entryContent = new EntryInfo()
            {
                SearchQuery = query.SearchQuery,
                Content = content.Substring(0, 300),
            };
            
            entriesContent.Add(entryContent);
        }

        return entriesContent;
    }
    
    public static async IAsyncEnumerable<EntryInfo> GetEntriesInfoAsync()
    {
        var repository = new Repository();
        var queries = await repository.GetAll();

        var entriesContent = new List<EntryInfo>();
        
        using var httpClient = new HttpClient();
        
        foreach (var query in queries)
        {
            var content = await httpClient.GetStringAsync("https://yandex.ru/search/?text=" + HttpUtility.UrlEncode(query.SearchQuery));

            var entryContent = new EntryInfo()
            {
                SearchQuery = query.SearchQuery,
                Content = content.Substring(0, 300),
            };

            yield return entryContent;
        }
    }

    public class Repository
    {
        private const string _path = @"Queries.txt"; 
        
        public async Task<IReadOnlyCollection<Query>> GetAll()
        {
            await using FileStream fileStream = File.OpenRead(_path);
            
            var result = await JsonSerializer.DeserializeAsync<Query[]>(fileStream);

            return result;
        }
    }

    public class Query
    {
        public string SearchQuery { get; set; }
    }
    
    public class EntryInfo
    {
        public string SearchQuery { get; set; }
        public string Content { get; set; }
    }
}