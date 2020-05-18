using StockView.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockView.UI.Data
{
    public interface IStockDataService
    {
        Task<IEnumerable<Stock>> GetAllAsync();
    }
}