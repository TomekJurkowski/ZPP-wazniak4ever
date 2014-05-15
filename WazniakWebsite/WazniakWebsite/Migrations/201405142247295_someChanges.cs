namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someChanges : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "dbo.Module", newSchema: "clarifier");
        }
        
        public override void Down()
        {
            MoveTable(name: "clarifier.Module", newSchema: "dbo");
        }
    }
}
