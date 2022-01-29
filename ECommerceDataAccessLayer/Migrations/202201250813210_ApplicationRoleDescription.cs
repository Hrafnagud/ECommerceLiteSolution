namespace ECommerceDataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationRoleDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "Description", c => c.String(maxLength: 200));
            DropColumn("dbo.AspNetRoles", "Desctiption");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetRoles", "Desctiption", c => c.String(maxLength: 200));
            DropColumn("dbo.AspNetRoles", "Description");
        }
    }
}
