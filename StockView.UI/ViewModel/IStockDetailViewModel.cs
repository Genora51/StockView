using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public interface IStockDetailViewModel
    {
        Task LoadAsync(int? stockId);
        bool HasChanges { get; }
    }
}