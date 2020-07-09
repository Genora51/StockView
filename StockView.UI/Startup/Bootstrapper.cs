using Autofac;
using Prism.Events;
using StockView.DataAccess;
using StockView.UI.Data.Lookups;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.ViewModel;

namespace StockView.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            // Prism Events
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            // Database
            builder.RegisterType<StockViewDbContext>().AsSelf();

            // MVVM
            builder.RegisterType<MainWindow>().AsSelf();

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<StockDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(StockDetailViewModel));
            builder.RegisterType<PageDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(PageDetailViewModel));
            builder.RegisterType<IndustryDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(IndustryDetailViewModel));
            builder.RegisterType<PageDataDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(PageDataDetailViewModel));

            // VM Services
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<StockRepository>().As<IStockRepository>();
            builder.RegisterType<PageRepository>().As<IPageRepository>();
            builder.RegisterType<IndustryRepository>().As<IIndustryRepository>();
            builder.RegisterType<PageDataRepository>().As<IPageDataRepository>();

            return builder.Build();
        }
    }
}
