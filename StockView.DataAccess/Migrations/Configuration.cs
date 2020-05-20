namespace StockView.DataAccess.Migrations
{
    using StockView.Model;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

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
                new Stock { CompanyName = "Alphabet Inc.", Symbol = "GOOGL" },
                new Stock { CompanyName = "Home Depot", Symbol = "HD" },
                new Stock { CompanyName = "Johnson & Johnson", Symbol = "JNJ" }
                );
            context.Industries.AddOrUpdate(
                i => i.Name,
                new Industry { Name = "Consumer Cyclical" },
                new Industry { Name = "Healthcare" },
                new Industry { Name = "Technology" }
                );
            context.SaveChanges();

            context.StockSnapshots.AddOrUpdate(ss => new { ss.Date, ss.StockId },
                new StockSnapshot { Date = new DateTime(2020, 5, 19), StockId = context.Stocks.First().Id });
        }
    }
}
