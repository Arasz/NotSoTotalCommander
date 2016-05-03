using GalaSoft.MvvmLight.Messaging;

namespace NotSoTotalCommanderApp.Messages
{
    /// <summary>
    /// </summary>
    public class CancelAsyncOperationMessage : MessageBase
    {
        public CancelAsyncOperationMessage()
        {
        }

        public CancelAsyncOperationMessage(object sender) : base(sender)
        {
        }

        public CancelAsyncOperationMessage(object sender, object target) : base(sender, target)
        {
        }
    }
}