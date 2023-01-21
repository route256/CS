namespace W2_1_AsyncAwait;

public class AsyncAwaitMain
{
    public static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        
        var data = await httpClient.GetStringAsync("http://ozon.ru");
        
        if (data.Length > 0)
        {
            Console.WriteLine(data.Substring(0, 300));
        }
    }
}


