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
            var possibleValue = await _stockWebService.GetValueAsync(stock.Symbol, date);
            decimal value = 0;
            if (possibleValue is decimal val)
                value = val;
            var exDate = await _stockWebService.GetExDividendsAsync(stock.Symbol);
            bool isExDividends = false;
            if (exDate is DateTime exDateValue)
                isExDividends = exDateValue.Date == date.Date;

            // TODO: better handling if API errs
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
