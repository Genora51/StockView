using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockView.UI.View.Services
{
    public interface IPrintService
    {
        void Print(DataTable data, DataTable summaryData, string title);
    }
}
