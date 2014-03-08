namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewModelsAddedAndAbcdAnswerReplaced : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MultiChoice",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ChoiceString = c.String(nullable: false, maxLength: 200),
                        ChoiceBool = c.Boolean(nullable: false),
                        MultipleChoiceAnswerID = c.Int(nullable: false),
                        MultipleChoiceAnswer_TaskID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("clarifier.Answer", t => t.MultipleChoiceAnswer_TaskID)
                .Index(t => t.MultipleChoiceAnswer_TaskID);
            
            CreateTable(
                "dbo.SingleChoice",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ChoiceString = c.String(nullable: false, maxLength: 200),
                        SingleChoiceAnswerID = c.Int(nullable: false),
                        SingleChoiceAnswer_TaskID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("clarifier.Answer", t => t.SingleChoiceAnswer_TaskID)
                .Index(t => t.SingleChoiceAnswer_TaskID);
            
            DropColumn("clarifier.Answer", "A");
            DropColumn("clarifier.Answer", "B");
            DropColumn("clarifier.Answer", "C");
            DropColumn("clarifier.Answer", "D");
        }
        
        public override void Down()
        {
            AddColumn("clarifier.Answer", "D", c => c.String());
            AddColumn("clarifier.Answer", "C", c => c.String());
            AddColumn("clarifier.Answer", "B", c => c.String());
            AddColumn("clarifier.Answer", "A", c => c.String());
            DropForeignKey("dbo.SingleChoice", "SingleChoiceAnswer_TaskID", "clarifier.Answer");
            DropForeignKey("dbo.MultiChoice", "MultipleChoiceAnswer_TaskID", "clarifier.Answer");
            DropIndex("dbo.SingleChoice", new[] { "SingleChoiceAnswer_TaskID" });
            DropIndex("dbo.MultiChoice", new[] { "MultipleChoiceAnswer_TaskID" });
            DropTable("dbo.SingleChoice");
            DropTable("dbo.MultiChoice");
        }
    }
}
