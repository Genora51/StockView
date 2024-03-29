﻿using StockView.Model;

namespace StockView.UI.Data.Repositories
{
    public interface IPageDataRepository : IGenericRepository<Page>
    {
        void DetachPage(Page page);
        void RemoveSnapshot(StockSnapshot Model);

    }
}
