// See https://aka.ms/new-console-template for more information

using RateLimiterCore;

public class Programm
{
    public static async Task Main(string[] args)
    {
        var random = new Random();
        
        var limiter = new RateLimiter<int>();

        var tasksList = new List<Task<Result<int>>>();

        for (int i = 0; i < 4; i++)
        {
            var task = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                
                return await limiter.Invoke(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                        
                        return random.Next();
                    }, 
                    CancellationToken.None);
            });
            
            tasksList.Add(task);
        }

        await Task.WhenAll(tasksList);
    }
}