namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StockPages",
                c => new
                    {
                        Stock_Id = c.Int(nullable: false),
                        Page_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Stock_Id, t.Page_Id })
                .ForeignKey("dbo.Stocks", t => t.Stock_Id, cascadeDelete: true)
                .ForeignKey("dbo.Pages", t => t.Page_Id, cascadeDelete: true)
                .Index(t => t.Stock_Id)
                .Index(t => t.Page_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockPages", "Page_Id", "dbo.Pages");
            DropForeignKey("dbo.StockPages", "Stock_Id", "dbo.Stocks");
            DropIndex("dbo.StockPages", new[] { "Page_Id" });
            DropIndex("dbo.StockPages", new[] { "Stock_Id" });
            DropTable("dbo.StockPages");
            DropTable("dbo.Pages");
        }
    }
}
