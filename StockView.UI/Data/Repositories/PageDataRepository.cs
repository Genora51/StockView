using StockView.DataAccess;
using StockView.Model;
using System.Threading.Tasks;
using System.Data.Entity;

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
                .SingleAsync(p => p.Id == id);
            var entry = Context.Entry(page);
            await entry.Collection(p => p.Stocks)
                .Query()
                .Include(s => s.Snapshots)
                .LoadAsync();
            return page;
        }
    }
}
