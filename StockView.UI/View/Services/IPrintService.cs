using System.Data;

namespace StockView.UI.View.Services
{
    public interface IPrintService
    {
        void Print(DataTable data, DataTable summaryData, string title);
    }
}
