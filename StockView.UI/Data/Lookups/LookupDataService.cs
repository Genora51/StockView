using StockView.DataAccess;
using StockView.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace StockView.UI.Data.Lookups
{
    public class LookupDataService : IStockLookupDataService,
        IIndustryLookupDataService,
        IPageLookupDataService,
        ISummaryLookupDataService
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

        public async Task<IEnumerable<LookupItem>> GetIndustryLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Industries.AsNoTracking()
                    .Select(i =>
                    new LookupItem
                    {
                        Id = i.Id,
                        DisplayMember = i.Name
                    })
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetPageLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                var items = await ctx.Pages.AsNoTracking()
                    .Select(p =>
                        new LookupItem
                        {
                            Id = p.Id,
                            DisplayMember = p.Title
                        })
                    .ToListAsync();
                return items;
            }
        }

        public async Task<IEnumerable<Summary>> GetSummaryLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Summaries.AsNoTracking().ToListAsync();
            }
        }
    }
}
