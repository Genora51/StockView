using System;
using System.Threading.Tasks;

namespace StockView.Fetch.Client
{
    public interface IStockWebServiceClient
    {
        Task<decimal?> GetValueAsync(string symbol, DateTime date);
        Task<DateTime?> GetExDividendsAsync(string symbol);
        Task<decimal?> GetYieldAsync(string symbol);
    }
}