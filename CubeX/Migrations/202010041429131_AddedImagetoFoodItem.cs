namespace CubeX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImagetoFoodItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FoodItems", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FoodItems", "Image");
        }
    }
}
