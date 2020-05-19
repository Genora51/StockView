namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStockSnapshots : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockSnapshots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        StockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stocks", t => t.StockId, cascadeDelete: true)
                .Index(t => t.StockId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockSnapshots", "StockId", "dbo.Stocks");
            DropIndex("dbo.StockSnapshots", new[] { "StockId" });
            DropTable("dbo.StockSnapshots");
        }
    }
}
