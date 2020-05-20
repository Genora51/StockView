using StockView.DataAccess;
using StockView.Model;
using System.Threading.Tasks;
using System.Data.Entity;

namespace StockView.UI.Data.Repositories
{
    public class PageRepository : GenericRepository<Page, StockViewDbContext>,
                                  IPageRepository
    {
        public PageRepository(StockViewDbContext context) : base(context)
        {
        }

        public async override Task<Page> GetByIdAsync(int id)
        {
            return await Context.Pages
                .Include(p => p.Stocks)
                .SingleAsync(p => p.Id == id);
        }
    }
}
