namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1to1RelationshipBetweenTaskAndAnswerAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("wazniak_forever.Task", "AnswerID", "wazniak_forever.Answer");
            DropIndex("wazniak_forever.Task", new[] { "AnswerID" });
            AddColumn("wazniak_forever.Answer", "TaskID", c => c.Int(nullable: false));
            DropPrimaryKey("wazniak_forever.Answer");
            AddPrimaryKey("wazniak_forever.Answer", "TaskID");
            CreateIndex("wazniak_forever.Answer", "TaskID");
            AddForeignKey("wazniak_forever.Answer", "TaskID", "wazniak_forever.Task", "ID");
            DropColumn("wazniak_forever.Answer", "ID");
        }
        
        public override void Down()
        {
            AddColumn("wazniak_forever.Answer", "ID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("wazniak_forever.Answer", "TaskID", "wazniak_forever.Task");
            DropIndex("wazniak_forever.Answer", new[] { "TaskID" });
            DropPrimaryKey("wazniak_forever.Answer");
            AddPrimaryKey("wazniak_forever.Answer", "ID");
            DropColumn("wazniak_forever.Answer", "TaskID");
            CreateIndex("wazniak_forever.Task", "AnswerID");
            AddForeignKey("wazniak_forever.Task", "AnswerID", "wazniak_forever.Answer", "ID", cascadeDelete: true);
        }
    }
}
