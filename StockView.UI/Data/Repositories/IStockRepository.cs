using StockView.Model;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public interface IStockRepository : IGenericRepository<Stock>
    {
        void RemoveSnapshot(StockSnapshot model);

        Task<bool> HasPagesAsync(int stockId);
        Task<bool> BelongsToPageAsync(int stockId, int pageId);
        void DetachStock(Stock stock);
    }
}