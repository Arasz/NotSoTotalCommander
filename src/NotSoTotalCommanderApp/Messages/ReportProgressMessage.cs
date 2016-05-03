using GalaSoft.MvvmLight.Messaging;

namespace NotSoTotalCommanderApp.Messages
{
    /// <summary>
    /// </summary>
    internal class ReportProgressMessage<T> : MessageBase
    {
        public T Progress { get; }

        public ReportProgressMessage(T progress)
        {
            Progress = progress;
        }
    }
}