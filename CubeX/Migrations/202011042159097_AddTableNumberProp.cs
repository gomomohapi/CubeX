namespace CubeX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableNumberProp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TableBookings", "TableNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TableBookings", "TableNumber");
        }
    }
}
