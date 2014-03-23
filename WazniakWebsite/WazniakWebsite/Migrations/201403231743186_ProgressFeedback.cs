namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProgressFeedback : DbMigration
    {
        public override void Up()
        {
            AddColumn("clarifier.Task", "CorrectAnswers", c => c.Int(nullable: false));
            AddColumn("clarifier.Task", "Attempts", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("clarifier.Task", "Attempts");
            DropColumn("clarifier.Task", "CorrectAnswers");
        }
    }
}
