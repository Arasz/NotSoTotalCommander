using Button = System.Windows.MessageBoxButton;

namespace NotSoTotalCommanderApp.Services
{
    internal interface IDialogService
    {
        /// <summary>
        /// Show message to user so that he will made decision 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="messageButton"></param>
        /// <param name="type"></param>
        DecisionResult<string> ShowDecisionMessage(string message, string title, Button messageButton, DecisionType type);

        /// <summary>
        /// Shows error message to user 
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="errorTitle"></param>
        /// <param name="messageButton"></param>
        void ShowError(string errorMessage, string errorTitle, Button messageButton);

        /// <summary>
        /// Shows information message to user 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="messageButton"></param>
        void ShowMessage(string message, string title, Button messageButton);

        /// <summary>
        /// Show operation progress message 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        void ShowProgressMessage(string message, string title);
    }
}