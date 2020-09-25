using Autofac;
using Prism.Events;
using StockView.DataAccess;
using StockView.Fetch;
using StockView.Fetch.Client;
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

            // API Fetch
            builder.Register(ctx =>
            {
                var apiKey = Properties.Settings.Default["api_key"] as string;
                return new StockWebServiceClient(apiKey);
            }).As<IStockWebServiceClient>();
            builder.RegisterType<StockDataFetchService>().As<IStockDataFetchService>();

            // MVVM
            builder.RegisterType<MainWindow>().AsSelf();

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            builder.RegisterType<PrintService>().As<IPrintService>();

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
            builder.RegisterType<SettingsDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(SettingsDetailViewModel));
            builder.RegisterType<SummaryDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(SummaryDetailViewModel));

            // VM Services
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<StockRepository>().As<IStockRepository>();
            builder.RegisterType<PageRepository>().As<IPageRepository>();
            builder.RegisterType<IndustryRepository>().As<IIndustryRepository>();
            builder.RegisterType<PageDataRepository>().As<IPageDataRepository>();
            builder.RegisterType<SummaryRepository>().As<ISummaryRepository>();

            return builder.Build();
        }
    }
}
