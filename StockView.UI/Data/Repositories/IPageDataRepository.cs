using StockView.Model;

namespace StockView.UI.Data.Repositories
{
    public interface IPageDataRepository : IGenericRepository<Page>
    {
        void RemoveSnapshot(StockSnapshot Model);
    }
}
