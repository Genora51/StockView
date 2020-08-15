namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSharesToStock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stocks", "Shares", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stocks", "Shares");
        }
    }
}
