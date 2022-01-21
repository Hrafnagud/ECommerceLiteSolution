namespace ECommerceDataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryTableUpdated : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Category", new[] { "BaseCategoryId" });
            AlterColumn("dbo.Category", "BaseCategoryId", c => c.Int());
            CreateIndex("dbo.Category", "BaseCategoryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Category", new[] { "BaseCategoryId" });
            AlterColumn("dbo.Category", "BaseCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Category", "BaseCategoryId");
        }
    }
}
