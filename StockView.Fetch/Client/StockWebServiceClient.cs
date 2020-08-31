using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace StockView.Fetch.Client
{
    public class StockWebServiceClient : IStockWebServiceClient
    {
        private const string APIUrl = @"https://cloud.iexapis.com/stable";
        private string _key;

        public StockWebServiceClient(string key)
        {
            _key = key;
        }

        public async Task<DateTime?> GetExDividendsAsync(string symbol)
        {
            var url = APIUrl
                .AppendPathSegments("stock", symbol, "dividends", "3m")
                .SetQueryParam("token", _key);
            IList<dynamic> result;
            try
            {
                result = await url.GetJsonListAsync();
            }
            catch (FlurlHttpException)
            {
                return null;
            }
            if (result.Count > 0)
            {
                if (DateTime.TryParseExact(result[0].exDate, "yyyy-MM-dd",
                        null, DateTimeStyles.None, out DateTime parsedDate))
                    return parsedDate;
                else return null;
            }
            else return null;
        }

        public async Task<decimal?> GetValueAsync(string symbol, DateTime date)
        {
            var url = APIUrl
                .AppendPathSegments("stock", symbol, "chart", "date", $"{date:yyyyMMdd}")
                .SetQueryParams(new
                {
                    token = _key,
                    chartByDay = "true"
                });
            try
            {
                var result = await url.GetJsonListAsync();
                if (result.Count > 0)
                {
                    return (decimal)result[0].uClose;
                }
                else return null;
            }
            catch (FlurlHttpException)
            {
                return null;
            }
        }
    }
}
