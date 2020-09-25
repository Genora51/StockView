using StockView.Model;
using System;
using System.Threading.Tasks;

namespace StockView.Fetch
{
    public interface IStockDataFetchService
    {
        Task<StockSnapshot> FetchSnapshotAsync(Stock stock, DateTime date);
        Task<decimal?> FetchYieldAsync(Stock stock);
    }
}