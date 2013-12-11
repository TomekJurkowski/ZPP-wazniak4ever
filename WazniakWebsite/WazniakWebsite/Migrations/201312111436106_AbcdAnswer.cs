namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AbcdAnswer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "wazniak_forever.AbcdAnswer",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        A = c.String(),
                        B = c.String(),
                        C = c.String(),
                        D = c.String(),
                        CorrectAnswer = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("wazniak_forever.AbcdAnswer");
        }
    }
}
