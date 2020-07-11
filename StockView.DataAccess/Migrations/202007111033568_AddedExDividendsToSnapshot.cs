namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedExDividendsToSnapshot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockSnapshots", "ExDividends", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockSnapshots", "ExDividends");
        }
    }
}
