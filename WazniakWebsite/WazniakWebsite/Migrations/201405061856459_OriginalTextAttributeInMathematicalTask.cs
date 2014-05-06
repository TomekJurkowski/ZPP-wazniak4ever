namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OriginalTextAttributeInMathematicalTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("clarifier.Task", "OriginalText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("clarifier.Task", "OriginalText");
        }
    }
}
