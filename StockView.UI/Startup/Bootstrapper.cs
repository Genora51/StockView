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

            builder.RegisterType<StockViewDbContext>().AsSelf();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<StockDataService>().As<IStockDataService>();

            return builder.Build();
        }
    }
}
