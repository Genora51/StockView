using StockView.DataAccess;
using StockView.Model;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public class PageDataRepository : GenericRepository<Page, StockViewDbContext>,
        IPageDataRepository
    {
        public PageDataRepository(StockViewDbContext context) : base(context)
        {
        }

        public async override Task<Page> GetByIdAsync(int id)
        {
            var page = await Context.Pages
                .Include(p => p.Stocks.Select(s => s.Snapshots))
                .Include(p => p.Stocks.Select(s => s.Industry))
                .SingleAsync(p => p.Id == id);
            return page;
        }

        public void RemoveSnapshot(StockSnapshot model)
        {
            Context.StockSnapshots.Remove(model);
        }

        public void DetachPage(Page page)
        {
            var changedEntriesCopy = Context.ChangeTracker.Entries();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
