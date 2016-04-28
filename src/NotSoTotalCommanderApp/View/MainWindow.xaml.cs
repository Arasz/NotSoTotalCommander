﻿using GalaSoft.MvvmLight.Messaging;
using NotSoTotalCommanderApp.Messages;
using System;
using System.Windows;

namespace NotSoTotalCommanderApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml 
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMessenger _messenger;

        public MainWindow()
        {
            InitializeComponent();

            _messenger = Messenger.Default;

            _messenger.Register<UserDecisionRequestMessage>(this, ResponseForUserDecisionRequest);
        }

        private void ResponseForUserDecisionRequest(UserDecisionRequestMessage message)
        {
            var userResponseMessage = new UserDecisionResultMessage();
            switch (message.DecisionType)
            {
                case DecisionType.DepthCopy:
                    var copyResult = MessageBox.Show(this, "Paste all files and subdirectories?", "Interaction",
                        MessageBoxButton.YesNoCancel);
                    userResponseMessage.UserDecisionResult.Enqueue(copyResult);
                    break;

                case DecisionType.Override:
                    var overrideResult = MessageBox.Show(this, "Copy all files and subdirectories?", "Interaction",
                        MessageBoxButton.YesNoCancel);
                    userResponseMessage.UserDecisionResult.Enqueue(overrideResult);
                    break;

                case DecisionType.Delete:
                    var deleteResult = MessageBox.Show(this, "Copy all files and subdirectories?", "Interaction",
                        MessageBoxButton.YesNoCancel);
                    userResponseMessage.UserDecisionResult.Enqueue(deleteResult);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _messenger.Send(userResponseMessage);
        }
    }
}