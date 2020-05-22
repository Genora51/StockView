using StockView.Model;
using System.Threading.Tasks;

namespace StockView.UI.Data.Repositories
{
    public interface IIndustryRepository
        : IGenericRepository<Industry>
    {
        Task<bool> IsReferencedByStockAsync(int industryId);
    }
}
