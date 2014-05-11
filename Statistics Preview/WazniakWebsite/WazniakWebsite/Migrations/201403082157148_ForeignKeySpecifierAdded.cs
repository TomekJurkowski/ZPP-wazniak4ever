namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeySpecifierAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MultiChoice", "MultipleChoiceAnswer_TaskID", "clarifier.Answer");
            DropForeignKey("dbo.SingleChoice", "SingleChoiceAnswer_TaskID", "clarifier.Answer");
            DropIndex("dbo.MultiChoice", new[] { "MultipleChoiceAnswer_TaskID" });
            DropIndex("dbo.SingleChoice", new[] { "SingleChoiceAnswer_TaskID" });
            DropColumn("dbo.MultiChoice", "MultipleChoiceAnswerID");
            DropColumn("dbo.SingleChoice", "SingleChoiceAnswerID");
            RenameColumn(table: "dbo.MultiChoice", name: "MultipleChoiceAnswer_TaskID", newName: "MultipleChoiceAnswerID");
            RenameColumn(table: "dbo.SingleChoice", name: "SingleChoiceAnswer_TaskID", newName: "SingleChoiceAnswerID");
            AlterColumn("dbo.MultiChoice", "MultipleChoiceAnswerID", c => c.Int(nullable: false));
            AlterColumn("dbo.SingleChoice", "SingleChoiceAnswerID", c => c.Int(nullable: false));
            CreateIndex("dbo.MultiChoice", "MultipleChoiceAnswerID");
            CreateIndex("dbo.SingleChoice", "SingleChoiceAnswerID");
            AddForeignKey("dbo.MultiChoice", "MultipleChoiceAnswerID", "clarifier.Answer", "TaskID", cascadeDelete: true);
            AddForeignKey("dbo.SingleChoice", "SingleChoiceAnswerID", "clarifier.Answer", "TaskID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SingleChoice", "SingleChoiceAnswerID", "clarifier.Answer");
            DropForeignKey("dbo.MultiChoice", "MultipleChoiceAnswerID", "clarifier.Answer");
            DropIndex("dbo.SingleChoice", new[] { "SingleChoiceAnswerID" });
            DropIndex("dbo.MultiChoice", new[] { "MultipleChoiceAnswerID" });
            AlterColumn("dbo.SingleChoice", "SingleChoiceAnswerID", c => c.Int());
            AlterColumn("dbo.MultiChoice", "MultipleChoiceAnswerID", c => c.Int());
            RenameColumn(table: "dbo.SingleChoice", name: "SingleChoiceAnswerID", newName: "SingleChoiceAnswer_TaskID");
            RenameColumn(table: "dbo.MultiChoice", name: "MultipleChoiceAnswerID", newName: "MultipleChoiceAnswer_TaskID");
            AddColumn("dbo.SingleChoice", "SingleChoiceAnswerID", c => c.Int(nullable: false));
            AddColumn("dbo.MultiChoice", "MultipleChoiceAnswerID", c => c.Int(nullable: false));
            CreateIndex("dbo.SingleChoice", "SingleChoiceAnswer_TaskID");
            CreateIndex("dbo.MultiChoice", "MultipleChoiceAnswer_TaskID");
            AddForeignKey("dbo.SingleChoice", "SingleChoiceAnswer_TaskID", "clarifier.Answer", "TaskID");
            AddForeignKey("dbo.MultiChoice", "MultipleChoiceAnswer_TaskID", "clarifier.Answer", "TaskID");
        }
    }
}
