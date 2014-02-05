namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Task : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "wazniak_forever.Task",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("wazniak_forever.Solution", "Title", c => c.String(maxLength: 50));
            AddColumn("wazniak_forever.Solution", "Text", c => c.String());
            DropTable("wazniak_forever.MathematicalTask");
        }
        
        public override void Down()
        {
            CreateTable(
                "wazniak_forever.MathematicalTask",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            DropColumn("wazniak_forever.Solution", "Text");
            DropColumn("wazniak_forever.Solution", "Title");
            DropTable("wazniak_forever.Task");
        }
    }
}
