﻿using StockView.DataAccess;
using StockView.Model;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections.Generic;

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

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await Context.Set<Stock>()
                .ToListAsync();
        }
    }
}
