namespace CubeX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TableBookings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        TableId = c.Int(nullable: false),
                        BookingMade = c.DateTime(),
                        BookingDate = c.DateTime(),
                        BookingTime = c.DateTime(),
                        Seats = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Image = c.Binary(),
                        Avaibility = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tables");
            DropTable("dbo.TableBookings");
        }
    }
}
