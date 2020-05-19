using StockView.DataAccess;
using StockView.Model;
using System;
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
        public async Task<Stock> GetByIdAsync(int stockId)
        {
            return await _context.Stocks.SingleAsync(s => s.Id == stockId);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
