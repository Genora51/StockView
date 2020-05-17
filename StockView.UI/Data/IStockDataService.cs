using StockView.Model;
using System.Collections.Generic;

namespace StockView.UI.Data
{
    public interface IStockDataService
    {
        IEnumerable<Stock> GetAll();
    }
}