using System.Data;

namespace StockView.UI.View.Services
{
    public interface IPrintService
    {
        void Print(DataView data, DataView summaryData, string title);
    }
}
