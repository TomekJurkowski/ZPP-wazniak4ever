namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskWithImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("clarifier.Task", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("clarifier.Task", "ImageUrl");
        }
    }
}
