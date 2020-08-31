using StockView.DataAccess;
using StockView.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
