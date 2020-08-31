using StockView.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockView.UI.Data.Lookups
{
    public interface ISummaryLookupDataService
    {
        Task<IEnumerable<Summary>> GetSummaryLookupAsync();
    }
}
