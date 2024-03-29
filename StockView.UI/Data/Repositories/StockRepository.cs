﻿using StockView.DataAccess;
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
            var stock = await Context.Stocks
                .SingleAsync(s => s.Id == stockId);
            var entry = Context.Entry(stock);
            await entry.Collection(s => s.Snapshots)
                .Query()
                .OrderBy(sn => sn.Date)
                .LoadAsync();
            return stock;
        }

        public async Task<bool> HasPagesAsync(int stockId)
        {
            return await Context.Pages.AsNoTracking()
                .Include(p => p.Stocks)
                .AnyAsync(p => p.Stocks.Any(s => s.Id == stockId));
        }

        public async Task<bool> BelongsToPageAsync(int stockId, int pageId)
        {
            var page = await Context.Pages.AsNoTracking()
                .Include(p => p.Stocks)
                .SingleOrDefaultAsync(p => p.Id == pageId);
            if (page == null) return false;
            return page.Stocks.Any(s => s.Id == stockId);
        }

        public void DetachStock(Stock stock)
        {
            foreach (var snapEntry in Context.ChangeTracker.Entries<StockSnapshot>())
            {
                snapEntry.State = EntityState.Detached;
            }
            var entry = Context.Entry(stock);
            entry.State = EntityState.Detached;
        }

        public void RemoveSnapshot(StockSnapshot model)
        {
            Context.StockSnapshots.Remove(model);
        }
    }
}
