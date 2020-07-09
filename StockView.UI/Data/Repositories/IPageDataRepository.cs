using StockView.Model;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public interface IPageDataRepository : IGenericRepository<Page>
    {
        void DetachPage(Page page);
        void RemoveSnapshot(StockSnapshot Model);
        
    }
}
