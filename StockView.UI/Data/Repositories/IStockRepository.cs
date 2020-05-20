using StockView.Model;

namespace StockView.UI.Data.Repositories
{
    public interface IStockRepository : IGenericRepository<Stock>
    {
        void RemoveSnapshot(StockSnapshot model);
    }
}