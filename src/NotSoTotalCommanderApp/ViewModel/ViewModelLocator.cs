/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:NotSoTotalCommanderApp"
                           x:Key="Locator" />
  </Application.Resources>

  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using NotSoTotalCommanderApp.Controls;
using NotSoTotalCommanderApp.Model;

namespace NotSoTotalCommanderApp.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the application and provides
    /// an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public FileSystemExplorerViewModel FileSystemExplorer
            => SimpleIoc.Default.GetInstanceWithoutCaching<FileSystemExplorerViewModel>();

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class. 
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<FileSystemExplorerModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FileSystemExplorerViewModel>();
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}