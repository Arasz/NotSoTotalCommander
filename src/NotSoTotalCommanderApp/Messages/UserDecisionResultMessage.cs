using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Windows;

namespace NotSoTotalCommanderApp.Messages
{
    internal class UserDecisionResultMessage : MessageBase
    {
        public Queue<MessageBoxResult> UserDecisionResult { get; } = new Queue<MessageBoxResult>();

        public UserDecisionResultMessage()
        {
        }
    }
}