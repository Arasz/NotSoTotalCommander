using GalaSoft.MvvmLight.Ioc;

namespace NotSoTotalCommanderApp.Utility
{
    /// <summary>
    /// Registers all types other than view models inside IoC 
    /// </summary>
    public class IocContainer
    {
        private SimpleIoc _container = new SimpleIoc();

        public SimpleIoc DefualtContainer => _container;

        public IocContainer()
        {
            Configure();
        }

        private void Configure()
        {
        }
    }
}