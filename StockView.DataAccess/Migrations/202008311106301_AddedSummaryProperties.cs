namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSummaryProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "Cost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Stocks", "Yield", c => c.Decimal(nullable: false, precision: 18, scale: 3));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stocks", "Yield");
            DropColumn("dbo.Stocks", "Cost");
        }
    }
}
