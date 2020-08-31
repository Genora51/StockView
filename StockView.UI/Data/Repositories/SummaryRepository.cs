using StockView.DataAccess;
using StockView.Model;

namespace StockView.UI.Data.Repositories
{
    public class SummaryRepository
        : GenericRepository<Summary, StockViewDbContext>,
        ISummaryRepository
    {
        public SummaryRepository(StockViewDbContext context)
            : base(context)
        {
        }
    }
}
