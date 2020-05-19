using StockView.Model;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public interface IStockRepository
    {
        Task<Stock> GetByIdAsync(int stockId);
        Task SaveAsync();
        bool HasChanges();
        void Add(Stock stock);
    }
}