using StockView.DataAccess;
using StockView.Model;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace StockView.UI.Data
{
    public class StockDataService : IStockDataService
    {
        private Func<StockViewDbContext> _contextCreator;

        public StockDataService(Func<StockViewDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }
        public async Task<Stock> GetByIdAsync(int stockId)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Stocks.AsNoTracking().SingleAsync(s => s.Id == stockId);
            }
        }
    }
}
