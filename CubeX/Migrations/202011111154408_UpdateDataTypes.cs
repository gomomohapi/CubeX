namespace CubeX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDataTypes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FoodItems", "Price", c => c.Double(nullable: false));
            AlterColumn("dbo.FoodItems", "TotalAmount", c => c.Double());
            AlterColumn("dbo.Orders", "TotalAmount", c => c.Double(nullable: false));
            AlterColumn("dbo.ShoppingCarts", "Sum", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShoppingCarts", "Sum", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Orders", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.FoodItems", "TotalAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.FoodItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
