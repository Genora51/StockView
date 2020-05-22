using StockView.DataAccess;
using StockView.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public class IndustryRepository
      : GenericRepository<Industry, StockViewDbContext>,
        IIndustryRepository
    {
        public IndustryRepository(StockViewDbContext context)
            : base(context)
        {
        }

        public async Task<bool> IsReferencedByStockAsync(int industryId)
        {
            return await Context.Stocks.AsNoTracking()
                .AnyAsync(s => s.IndustryId == industryId);
        }
    }
}
