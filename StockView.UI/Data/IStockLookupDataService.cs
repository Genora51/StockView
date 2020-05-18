using StockView.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockView.UI.Data
{
    public interface IStockLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetStockLookupAsync();
    }
}