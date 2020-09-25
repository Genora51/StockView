using StockView.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockView.UI.Data.Lookups
{
    public interface ISummaryLookupDataService
    {
        Task<IEnumerable<Summary>> GetSummaryLookupAsync();
    }
}
