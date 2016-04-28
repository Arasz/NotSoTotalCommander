using GalaSoft.MvvmLight.Messaging;

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

    internal enum DecisionType
    {
        DepthPaste,
        Override,
        Delete,
        Create,
    }
}