namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIndustry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Industries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Stocks", "IndustryId", c => c.Int());
            CreateIndex("dbo.Stocks", "IndustryId");
            AddForeignKey("dbo.Stocks", "IndustryId", "dbo.Industries", "Id");
            DropColumn("dbo.Stocks", "Industry");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stocks", "Industry", c => c.String(maxLength: 50));
            DropForeignKey("dbo.Stocks", "IndustryId", "dbo.Industries");
            DropIndex("dbo.Stocks", new[] { "IndustryId" });
            DropColumn("dbo.Stocks", "IndustryId");
            DropTable("dbo.Industries");
        }
    }
}
