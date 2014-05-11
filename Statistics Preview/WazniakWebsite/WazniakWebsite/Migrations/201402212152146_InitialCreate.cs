namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "wazniak_forever.Answer",
                c => new
                    {
                        TaskID = c.Int(nullable: false),
                        A = c.String(),
                        B = c.String(),
                        C = c.String(),
                        D = c.String(),
                        CorrectAnswer = c.Int(),
                        Value = c.String(maxLength: 100),
                        Text = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("wazniak_forever.Task", t => t.TaskID)
                .Index(t => t.TaskID);
            
            CreateTable(
                "wazniak_forever.Task",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        SubjectID = c.Int(nullable: false),
                        AnswerID = c.Int(nullable: false),
                        Text = c.String(),
                        Text1 = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("wazniak_forever.Subject", t => t.SubjectID, cascadeDelete: true)
                .Index(t => t.SubjectID);
            
            CreateTable(
                "wazniak_forever.Subject",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "wazniak_forever.SampleItem",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Number = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("wazniak_forever.Task", "SubjectID", "wazniak_forever.Subject");
            DropForeignKey("wazniak_forever.Answer", "TaskID", "wazniak_forever.Task");
            DropIndex("wazniak_forever.Task", new[] { "SubjectID" });
            DropIndex("wazniak_forever.Answer", new[] { "TaskID" });
            DropTable("wazniak_forever.SampleItem");
            DropTable("wazniak_forever.Subject");
            DropTable("wazniak_forever.Task");
            DropTable("wazniak_forever.Answer");
        }
    }
}
