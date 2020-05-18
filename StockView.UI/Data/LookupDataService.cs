using StockView.DataAccess;
using StockView.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace StockView.UI.Data
{
    public class LookupDataService : IStockLookupDataService
    {
        private Func<StockViewDbContext> _contextCreator;

        public LookupDataService(Func<StockViewDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetStockLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Stocks.AsNoTracking()
                    .Select(s =>
                    new LookupItem
                    {
                        Id = s.Id,
                        DisplayMember = s.Symbol
                    })
                    .ToListAsync();
            }
        }
    }
}
