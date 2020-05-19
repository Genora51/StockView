using StockView.DataAccess;
using StockView.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public class StockRepository : IStockRepository
    {
        private StockViewDbContext _context;

        public StockRepository(StockViewDbContext context)
        {
            _context = context;
        }

        public void Add(Stock stock)
        {
            _context.Stocks.Add(stock);
        }

        public async Task<Stock> GetByIdAsync(int stockId)
        {
            return await _context.Stocks.SingleAsync(s => s.Id == stockId);
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Remove(Stock stock)
        {
            _context.Stocks.Remove(stock);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
