using Nito.AsyncEx;

namespace CloudHolic.Utils.Extensions;

public static class AsyncAutoResetEventExtensions
{
    public static async Task<bool> WaitAsync(this AsyncAutoResetEvent source, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (timeout < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeout));

        if (timeout == Timeout.InfiniteTimeSpan)
        {
            await source.WaitAsync(cancellationToken);
            return true;
        }

        cancellationToken.ThrowIfCancellationRequested();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(timeout);

        try
        {
            await source.WaitAsync(linkedCts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return false;
        }
    }
}
