using StockView.DataAccess;
using StockView.Model;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

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
                .SingleAsync(p => p.Id == id);
            return page;
        }
    }
}
