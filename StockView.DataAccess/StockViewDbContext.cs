using StockView.Model;
using System.Data.Entity;

namespace StockView.DataAccess
{
    public class StockViewDbContext : DbContext
    {
        public StockViewDbContext() : base("StockViewDb")
        {

        }
        public DbSet<Stock> Stocks { get; set; }
    }
}
