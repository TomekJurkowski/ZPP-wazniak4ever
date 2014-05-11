namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SmallChangeInTaskModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("clarifier.Task", "AnswerID");
        }
        
        public override void Down()
        {
            AddColumn("clarifier.Task", "AnswerID", c => c.Int(nullable: false));
        }
    }
}
