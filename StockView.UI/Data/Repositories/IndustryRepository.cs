using StockView.DataAccess;
using StockView.Model;

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
    }
}
