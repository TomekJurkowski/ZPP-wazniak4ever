namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OriginalTextAttrChangedIntoModifiedText : DbMigration
    {
        public override void Up()
        {
            AddColumn("clarifier.Task", "ModifiedText", c => c.String());
            DropColumn("clarifier.Task", "OriginalText");
        }
        
        public override void Down()
        {
            AddColumn("clarifier.Task", "OriginalText", c => c.String());
            DropColumn("clarifier.Task", "ModifiedText");
        }
    }
}
