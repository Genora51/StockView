using StockView.DataAccess;
using StockView.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockView.UI.Data
{
    public class StockDataService : IStockDataService
    {
        private Func<StockViewDbContext> _contextCreator;

        public StockDataService(Func<StockViewDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }
        public IEnumerable<Stock> GetAll()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Stocks.AsNoTracking().ToList();
            }
        }
    }
}
