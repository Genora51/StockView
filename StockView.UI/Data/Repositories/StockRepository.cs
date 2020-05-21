using StockView.DataAccess;
using StockView.Model;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public class StockRepository : GenericRepository<Stock, StockViewDbContext>,
                                   IStockRepository
    {

        public StockRepository(StockViewDbContext context) : base(context)
        {
        }

        public override async Task<Stock> GetByIdAsync(int stockId)
        {
            return await Context.Stocks
                .Include(s => s.Snapshots)
                .SingleAsync(s => s.Id == stockId);
        }

        public async Task<bool> HasPagesAsync(int stockId)
        {
            return await Context.Pages.AsNoTracking()
                .Include(p => p.Stocks)
                .AnyAsync(p => p.Stocks.Any(s => s.Id == stockId));
        }

        public void RemoveSnapshot(StockSnapshot model)
        {
            Context.StockSnapshots.Remove(model);
        }
    }
}
