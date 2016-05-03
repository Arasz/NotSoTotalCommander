using GalaSoft.MvvmLight.Messaging;

namespace NotSoTotalCommanderApp.Messages
{
    /// <summary>
    /// </summary>
    internal class AsyncOperationIndicatorMessage : MessageBase
    {
        public bool CancelIndicator { get; }

        public AsyncOperationIndicatorMessage(bool cancelIndicator = false)
        {
            CancelIndicator = cancelIndicator;
        }

        public AsyncOperationIndicatorMessage(object sender) : base(sender)
        {
        }

        public AsyncOperationIndicatorMessage(object sender, object target) : base(sender, target)
        {
        }
    }
}