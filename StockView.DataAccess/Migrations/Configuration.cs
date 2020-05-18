namespace StockView.DataAccess.Migrations
{
    using StockView.Model;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<StockView.DataAccess.StockViewDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(StockView.DataAccess.StockViewDbContext context)
        {
            context.Stocks.AddOrUpdate(
                s => s.Symbol,
                new Stock { CompanyName = "Alphabet Inc.", Symbol = "GOOGL", Industry = "Technology" },
                new Stock { CompanyName = "Home Depot", Symbol = "HD", Industry = "Consumer Cyclical" },
                new Stock { CompanyName = "Johnson & Johnson", Symbol = "JNJ", Industry = "Healthcare" }
                );
        }
    }
}
