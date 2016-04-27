using GalaSoft.MvvmLight.Command;
using NotSoTotalCommanderApp.ViewModel;
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
                            actionType = ActionType.Paste;
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

            return actionType;
        }
    }
}