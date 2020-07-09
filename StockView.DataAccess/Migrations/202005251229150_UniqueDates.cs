namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueDates : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StockSnapshots", new[] { "StockId" });
            CreateIndex("dbo.StockSnapshots", new[] { "StockId", "Date" }, unique: true, name: "IX_StockDate");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockSnapshots", "IX_StockDate");
            CreateIndex("dbo.StockSnapshots", "StockId");
        }
    }
}
