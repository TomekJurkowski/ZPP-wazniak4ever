namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AbcdTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("wazniak_forever.Task", "Title", c => c.String(maxLength: 50));
            AddColumn("wazniak_forever.Task", "Text", c => c.String());
            AddColumn("wazniak_forever.Task", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("wazniak_forever.Task", "Discriminator");
            DropColumn("wazniak_forever.Task", "Text");
            DropColumn("wazniak_forever.Task", "Title");
        }
    }
}
