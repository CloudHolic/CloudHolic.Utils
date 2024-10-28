namespace CloudHolic.Utils;

public static class TaskEx
{
    public static async Task WaitWhile(Func<bool> condition, int timeout = -1, int frequency = 25)
    {
        var waitTask = Task.Run(async () =>
        {
            while (condition())
                await Task.Delay(frequency);
        });

        if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
            throw new TimeoutException();
    }

    public static async Task WaitUntil(Func<bool> condition, int timeout = -1, int frequency = 25)
    {
        var waitTask = Task.Run(async () =>
        {
            while (!condition())
                await Task.Delay(frequency);
        });

        if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
            throw new TimeoutException();
    }

    public static void SwallowException(this Task task) => task.ContinueWith(_ => { });
}
