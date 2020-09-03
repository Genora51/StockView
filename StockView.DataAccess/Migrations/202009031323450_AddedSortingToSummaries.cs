namespace StockView.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSortingToSummaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Summaries", "SortIndex", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Summaries", "SortIndex");
        }
    }
}
