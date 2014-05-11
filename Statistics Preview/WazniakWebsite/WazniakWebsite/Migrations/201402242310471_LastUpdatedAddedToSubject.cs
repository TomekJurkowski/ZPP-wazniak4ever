namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastUpdatedAddedToSubject : DbMigration
    {
        public override void Up()
        {
            AddColumn("clarifier.Subject", "LastUpdated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("clarifier.Subject", "LastUpdated");
        }
    }
}
