namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AbcdTaskDeleted : DbMigration
    {
        public override void Up()
        {
            DropColumn("wazniak_forever.Task", "Title");
            DropColumn("wazniak_forever.Task", "Text");
            DropColumn("wazniak_forever.Task", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("wazniak_forever.Task", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("wazniak_forever.Task", "Text", c => c.String());
            AddColumn("wazniak_forever.Task", "Title", c => c.String(maxLength: 50));
        }
    }
}
