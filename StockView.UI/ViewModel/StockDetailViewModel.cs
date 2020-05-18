using StockView.Model;
using StockView.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class StockDetailViewModel : ViewModelBase, IStockDetailViewModel
    {
        private IStockDataService _dataService;

        public StockDetailViewModel(IStockDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task LoadAsync(int stockId)
        {
            Stock = await _dataService.GetByIdAsync(stockId);
        }

        private Stock _stock;

        public Stock Stock
        {
            get { return _stock; }
            private set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }

    }
}
