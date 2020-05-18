using StockView.Model;
using StockView.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel(INavigationViewModel navigationViewModel,
            IStockDetailViewModel stockDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            StockDetailViewModel = stockDetailViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public INavigationViewModel NavigationViewModel { get; }
        public IStockDetailViewModel StockDetailViewModel { get; }
    }
}
