using StockView.Model;
using StockView.Model.Attributes;
using System;
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

        public DbSet<Summary> Summaries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new DataTypePropertyAttributeConvention());
            modelBuilder.Conventions.Add(new DecimalPrecisionAttributeConvention());
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

    public class DecimalPrecisionAttributeConvention
    : PrimitivePropertyAttributeConfigurationConvention<DecimalPrecisionAttribute>
    {
        public override void Apply(ConventionPrimitivePropertyConfiguration configuration, DecimalPrecisionAttribute attribute)
        {
            if (attribute.Precision < 1 || attribute.Precision > 38)
            {
                throw new InvalidOperationException("Precision must be between 1 and 38.");
            }

            if (attribute.Scale > attribute.Precision)
            {
                throw new InvalidOperationException("Scale must be between 0 and the Precision value.");
            }

            configuration.HasPrecision(attribute.Precision, attribute.Scale);
        }
    }
}
