namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnusedSampleItemsRemoved : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "wazniak_forever.Answer", newSchema: "clarifier");
            MoveTable(name: "wazniak_forever.Task", newSchema: "clarifier");
            MoveTable(name: "wazniak_forever.Subject", newSchema: "clarifier");
            DropTable("wazniak_forever.SampleItem");
        }
        
        public override void Down()
        {
            CreateTable(
                "wazniak_forever.SampleItem",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Number = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            MoveTable(name: "clarifier.Subject", newSchema: "wazniak_forever");
            MoveTable(name: "clarifier.Task", newSchema: "wazniak_forever");
            MoveTable(name: "clarifier.Answer", newSchema: "wazniak_forever");
        }
    }
}
