using GalaSoft.MvvmLight.Command;
using NotSoTotalCommanderApp.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotSoTotalCommanderApp.Converters
{
    internal class EventArgsToActionTypeConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            var actionType = ActionType.None;

            if (value is KeyEventArgs)
            {
                var keyEventArgs = (KeyEventArgs)value;

                keyEventArgs.Handled = true;

                switch (keyEventArgs.Key)
                {
                    case Key.Enter:
                        actionType = ActionType.OpenFileSystemItem;
                        break;

                    case Key.C:
                        if (keyEventArgs.KeyboardDevice.Modifiers == ModifierKeys.Control)
                            actionType = ActionType.Copy;
                        break;

                    case Key.V:
                        if (keyEventArgs.KeyboardDevice.Modifiers == ModifierKeys.Control)
                            actionType = ActionType.MoveOrPaste;
                        break;

                    case Key.Delete:
                        actionType = ActionType.Delete;
                        break;

                    case Key.X:
                        if (keyEventArgs.KeyboardDevice.Modifiers == ModifierKeys.Control)
                            actionType = ActionType.Cut;
                        break;
                }
            }
            else if (value is MouseButtonEventArgs)
            {
                var mouseEventsArgs = (MouseButtonEventArgs)value;

                mouseEventsArgs.Handled = true;

                if (mouseEventsArgs.ChangedButton == MouseButton.Left && mouseEventsArgs.ClickCount >= 1)
                    actionType = ActionType.OpenFileSystemItem;
            }
            else if (value is RoutedEventArgs)
            {
                var routedEventArgs = (RoutedEventArgs)value;
                var operation = (string)parameter;

                routedEventArgs.Handled = true;

                switch (((MenuItem)routedEventArgs.Source).Header.ToString())
                {
                    case "Copy":
                        actionType = ActionType.Copy;
                        break;

                    case "Paste":
                        actionType = ActionType.MoveOrPaste;
                        break;

                    case "Delete":
                        actionType = ActionType.Delete;
                        break;

                    case "Create":
                        actionType = ActionType.Create;
                        break;

                    case "Cut":
                        actionType = ActionType.Cut;
                        break;
                }
            }

            return actionType;
        }
    }
}