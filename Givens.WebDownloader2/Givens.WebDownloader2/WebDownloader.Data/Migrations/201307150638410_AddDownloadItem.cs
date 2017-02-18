namespace PhillipScottGivens.WebDownloader.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDownloadItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DownloadItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        Html = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DownloadItems");
        }
    }
}
