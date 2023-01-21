using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Ozon.Route256.Postgres.Domain.Common;

public static class AsyncEnumerableExtensions
{
    public static IAsyncEnumerable<IReadOnlyList<T>> Buffer<T>(this IAsyncEnumerable<T> source, int count, TimeSpan delay = default)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (count <= 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        return AsyncEnumerable.Create(BufferCore);

        async IAsyncEnumerator<IReadOnlyList<T>> BufferCore(CancellationToken cancellationToken)
        {
            var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, default);
            var enumerator = source
                .WithCancellation(cancellationSource.Token)
                .ConfigureAwait(false)
                .GetAsyncEnumerator();

            var enumeratorTask = default(ConfiguredValueTaskAwaitable<bool>);
            var buffer = new List<T>();

            try
            {
                while (true)
                {
                    var task = enumerator.MoveNextAsync();
                    var awaiter = task.GetAwaiter();
                    if (awaiter.IsCompleted)
                        goto GetResult;

                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, cancellationToken);
                        if (awaiter.IsCompleted)
                            goto GetResult;
                    }

                    if (buffer.Count > 0)
                    {
                        enumeratorTask = task;

                        yield return buffer.ToArray();

                        enumeratorTask = default;
                        buffer.Clear();
                    }

                    if (await task)
                        buffer.Add(enumerator.Current);
                    else
                        yield break;
                    continue;

                    GetResult:
                    if (awaiter.GetResult())
                    {
                        buffer.Add(enumerator.Current);

                        if (buffer.Count < count)
                            continue;

                        yield return buffer.ToArray();
                        buffer.Clear();
                    }
                    else
                    {
                        if (buffer.Count > 0)
                            yield return buffer;

                        yield break;
                    }
                }
            }
            finally
            {
                try
                {
                    cancellationSource.Cancel();
                    await enumeratorTask;
                }
                catch
                {
                    // We are not interested in any exceptions here
                    // because the fetch operation was scheduled while
                    // no one expects it.
                }

                cancellationSource.Dispose();
                await enumerator.DisposeAsync();
            }
        }
    }
}
