using System;
using System.Threading;

namespace NotSoTotalCommanderApp.Utility
{
    /// <summary>
    /// Wrapper for resources necessary to progress report and cancellation 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncOperationResources<T>
    {
        public CancellationTokenSource CancellationTokenSource { get; }

        public IProgress<T> Progress { get; }

        public AsyncOperationResources(IProgress<T> progress, CancellationTokenSource cancellationTokenSource)
        {
            Progress = progress;
            CancellationTokenSource = cancellationTokenSource;
        }
    }
}