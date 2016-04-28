using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Windows;

namespace NotSoTotalCommanderApp.Messages
{
    internal class UserDecisionResultMessage : MessageBase
    {
        public string Name { get; set; }

        public Queue<MessageBoxResult> UserDecisionResult { get; } = new Queue<MessageBoxResult>();

        public UserDecisionResultMessage()
        {
        }
    }
}