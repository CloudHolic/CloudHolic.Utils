using System.Runtime.CompilerServices;

namespace CloudHolic.Utils.Extensions;

public static class ThreadExtensions
{
    public static TaskAwaiter GetAwaiter(this Thread thread) =>
        Task.Run(async () =>
        {
            while (thread.IsAlive)
                await Task.Delay(100).ConfigureAwait(false);

            thread.Join();
        }).GetAwaiter();
}
