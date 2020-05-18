using StockView.Model;
using System.Threading.Tasks;

namespace StockView.UI.Data
{
    public interface IStockDataService
    {
        Task<Stock> GetByIdAsync(int stockId);
        Task SaveAsync(Stock stock);
    }
}