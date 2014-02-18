namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelRelationsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("wazniak_forever.Task", "SubjectID", c => c.Int(nullable: false));
            AddColumn("wazniak_forever.Task", "AnswerID", c => c.Int(nullable: false));
            CreateIndex("wazniak_forever.Task", "AnswerID");
            CreateIndex("wazniak_forever.Task", "SubjectID");
            AddForeignKey("wazniak_forever.Task", "AnswerID", "wazniak_forever.Answer", "ID", cascadeDelete: true);
            AddForeignKey("wazniak_forever.Task", "SubjectID", "wazniak_forever.Subject", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("wazniak_forever.Task", "SubjectID", "wazniak_forever.Subject");
            DropForeignKey("wazniak_forever.Task", "AnswerID", "wazniak_forever.Answer");
            DropIndex("wazniak_forever.Task", new[] { "SubjectID" });
            DropIndex("wazniak_forever.Task", new[] { "AnswerID" });
            DropColumn("wazniak_forever.Task", "AnswerID");
            DropColumn("wazniak_forever.Task", "SubjectID");
        }
    }
}
