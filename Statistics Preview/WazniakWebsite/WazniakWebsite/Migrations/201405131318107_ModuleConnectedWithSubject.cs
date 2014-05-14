namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleConnectedWithSubject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Module",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        SequenceNo = c.Int(nullable: false),
                        SubjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("clarifier.Subject", t => t.SubjectID, cascadeDelete: true)
                .Index(t => t.SubjectID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Module", "SubjectID", "clarifier.Subject");
            DropIndex("dbo.Module", new[] { "SubjectID" });
            DropTable("dbo.Module");
        }
    }
}
