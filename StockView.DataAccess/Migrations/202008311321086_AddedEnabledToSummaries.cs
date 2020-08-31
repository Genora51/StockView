namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEnabledToSummaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Summaries", "Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Summaries", "Enabled");
        }
    }
}
