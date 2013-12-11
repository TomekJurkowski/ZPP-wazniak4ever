namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validators_changed : DbMigration
    {
        public override void Up()
        {
            AlterColumn("wazniak_forever.AbcdAnswer", "A", c => c.String(nullable: false));
            AlterColumn("wazniak_forever.AbcdAnswer", "B", c => c.String(nullable: false));
            AlterColumn("wazniak_forever.AbcdAnswer", "C", c => c.String(nullable: false));
            AlterColumn("wazniak_forever.AbcdAnswer", "D", c => c.String(nullable: false));
            AlterColumn("wazniak_forever.Subject", "Name", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("wazniak_forever.Subject", "Name", c => c.String(maxLength: 50));
            AlterColumn("wazniak_forever.AbcdAnswer", "D", c => c.String());
            AlterColumn("wazniak_forever.AbcdAnswer", "C", c => c.String());
            AlterColumn("wazniak_forever.AbcdAnswer", "B", c => c.String());
            AlterColumn("wazniak_forever.AbcdAnswer", "A", c => c.String());
        }
    }
}
