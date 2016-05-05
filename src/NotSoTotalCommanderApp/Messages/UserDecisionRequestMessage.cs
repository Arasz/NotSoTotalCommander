using GalaSoft.MvvmLight.Messaging;
using NotSoTotalCommanderApp.Services;

namespace NotSoTotalCommanderApp.Messages
{
    internal class UserDecisionRequestMessage : MessageBase
    {
        public DecisionType DecisionType { get; }

        public UserDecisionRequestMessage(DecisionType decisionType)
        {
            DecisionType = decisionType;
        }
    }
}