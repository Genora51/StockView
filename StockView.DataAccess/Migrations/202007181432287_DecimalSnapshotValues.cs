namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecimalSnapshotValues : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StockSnapshots", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StockSnapshots", "Value", c => c.Single(nullable: false));
        }
    }
}
