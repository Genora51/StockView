using Autofac;
using StockView.DataAccess;
using StockView.UI.Data;
using StockView.UI.ViewModel;

namespace StockView.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            // Database
            builder.RegisterType<StockViewDbContext>().AsSelf();

            // MVVM
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();

            // VM Services
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<StockDataService>().As<IStockDataService>();

            return builder.Build();
        }
    }
}
