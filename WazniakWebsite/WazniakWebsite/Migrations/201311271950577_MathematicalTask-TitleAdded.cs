namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MathematicalTaskTitleAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("wazniak_forever.MathematicalTask", "Title", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("wazniak_forever.MathematicalTask", "Title");
        }
    }
}
