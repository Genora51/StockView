using StockView.Fetch.Client;
using StockView.Model;
using System;
using System.Threading.Tasks;

namespace StockView.Fetch
{
    public class StockDataFetchService : IStockDataFetchService
    {
        private IStockWebServiceClient _stockWebService;

        public StockDataFetchService(IStockWebServiceClient stockWebService)
        {
            _stockWebService = stockWebService;
        }

        public async Task<StockSnapshot> FetchSnapshotAsync(Stock stock, DateTime date)
        {
            var value = await _stockWebService.GetValueAsync(stock.Symbol, date);
            if (!value.HasValue) return null;
            var exDate = await _stockWebService.GetExDividendsAsync(stock.Symbol);
            bool isExDividends = false;
            if (exDate is DateTime exDateValue)
                isExDividends = exDateValue.Date == date.Date;

            return new StockSnapshot
            {
                StockId = stock.Id,
                Date = date.Date,
                ExDividends = isExDividends,
                Value = (float)value
            };
        }
    }
}
