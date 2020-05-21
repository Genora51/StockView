using StockView.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public interface IPageRepository : IGenericRepository<Page>
    {
        Task<IEnumerable<Stock>> GetAllStocksAsync();
    }
}