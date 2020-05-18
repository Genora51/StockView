using StockView.Model;
using System.Collections.Generic;
namespace StockView.UI.Data
{
    public class StockDataService : IStockDataService
    {
        public IEnumerable<Stock> GetAll()
        {
            // TODO: Load data from real database
            yield return new Stock { CompanyName = "Alphabet Inc.", Symbol = "GOOGL", Industry = "Technology" };
            yield return new Stock { CompanyName = "Home Depot", Symbol = "HD", Industry = "Consumer Cyclical" };
            yield return new Stock { CompanyName = "Johnson & Johnson", Symbol = "JNJ", Industry = "Healthcare" };
        }
    }
}
