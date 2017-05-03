namespace VirtoCommerce.CacheModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LastModified",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Scope = c.String(maxLength: 256),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Scope, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.LastModified", new[] { "Scope" });
            DropTable("dbo.LastModified");
        }
    }
}
