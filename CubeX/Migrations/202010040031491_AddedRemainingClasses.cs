namespace CubeX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRemainingClasses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Destination = c.String(maxLength: 50),
                        OrderDate = c.DateTime(nullable: false),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.ShoppingCarts",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Sum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApplicationUserID = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.FoodItems", "Quantity", c => c.Int());
            AddColumn("dbo.FoodItems", "TotalAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.FoodItems", "OrderID", c => c.String(maxLength: 128));
            AddColumn("dbo.FoodItems", "ShoppingCartID", c => c.String(maxLength: 128));
            AddColumn("dbo.FoodItems", "Discriminator1", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetUsers", "ShoppingCartID", c => c.String(maxLength: 128));
            CreateIndex("dbo.FoodItems", "OrderID");
            CreateIndex("dbo.FoodItems", "ShoppingCartID");
            CreateIndex("dbo.AspNetUsers", "ShoppingCartID");
            AddForeignKey("dbo.FoodItems", "OrderID", "dbo.Orders", "ID");
            AddForeignKey("dbo.FoodItems", "ShoppingCartID", "dbo.ShoppingCarts", "ID");
            AddForeignKey("dbo.AspNetUsers", "ShoppingCartID", "dbo.ShoppingCarts", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "ShoppingCartID", "dbo.ShoppingCarts");
            DropForeignKey("dbo.FoodItems", "ShoppingCartID", "dbo.ShoppingCarts");
            DropForeignKey("dbo.FoodItems", "OrderID", "dbo.Orders");
            DropIndex("dbo.AspNetUsers", new[] { "ShoppingCartID" });
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropIndex("dbo.FoodItems", new[] { "ShoppingCartID" });
            DropIndex("dbo.FoodItems", new[] { "OrderID" });
            DropColumn("dbo.AspNetUsers", "ShoppingCartID");
            DropColumn("dbo.FoodItems", "Discriminator1");
            DropColumn("dbo.FoodItems", "ShoppingCartID");
            DropColumn("dbo.FoodItems", "OrderID");
            DropColumn("dbo.FoodItems", "TotalAmount");
            DropColumn("dbo.FoodItems", "Quantity");
            DropTable("dbo.ShoppingCarts");
            DropTable("dbo.Orders");
        }
    }
}
