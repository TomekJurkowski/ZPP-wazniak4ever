namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleConnectedWithTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("clarifier.Task", "ModuleID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("clarifier.Task", "ModuleID");
        }
    }
}
