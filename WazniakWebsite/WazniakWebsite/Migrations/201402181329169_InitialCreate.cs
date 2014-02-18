namespace WazniakWebsite.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "wazniak_forever.Answer",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        A = c.String(),
                        B = c.String(),
                        C = c.String(),
                        D = c.String(),
                        CorrectAnswer = c.Int(),
                        Value = c.String(maxLength: 100),
                        Text = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "wazniak_forever.Task",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        Text = c.String(),
                        Title1 = c.String(maxLength: 50),
                        Text1 = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "wazniak_forever.SampleItem",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Number = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "wazniak_forever.Subject",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("wazniak_forever.Subject");
            DropTable("wazniak_forever.SampleItem");
            DropTable("wazniak_forever.Task");
            DropTable("wazniak_forever.Answer");
        }
    }
}
