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
        public CancellationToken CancellationToken { get; }

        public IProgress<T> Progress { get; }

        public AsyncOperationResources(IProgress<T> progress, ref CancellationToken cancellationToken)
        {
            Progress = progress;
            CancellationToken = cancellationToken;
        }
    }
}