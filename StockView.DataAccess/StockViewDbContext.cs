using StockView.Model;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace StockView.DataAccess
{
    public class StockViewDbContext : DbContext
    {
        public StockViewDbContext() : base("StockViewDb")
        {

        }
        public DbSet<Stock> Stocks { get; set; }

        public DbSet<Industry> Industries { get; set; }

        public DbSet<StockSnapshot> StockSnapshots { get; set; }

        public DbSet<Page> Pages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new DataTypePropertyAttributeConvention());
        }
    }

    public class DataTypePropertyAttributeConvention
        : PrimitivePropertyAttributeConfigurationConvention<DataTypeAttribute>
    {
        public override void Apply(ConventionPrimitivePropertyConfiguration configuration,
            DataTypeAttribute attribute)
        {
            if (attribute.DataType == DataType.Date)
            {
                configuration.HasColumnType("Date");
            }
        }
    }
}
