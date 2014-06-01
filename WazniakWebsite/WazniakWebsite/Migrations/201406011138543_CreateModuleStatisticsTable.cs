namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModuleStatisticsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "clarifier.ModuleStatistics",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ModuleID = c.Int(nullable: false),
                        CorrectAnswers = c.Int(nullable: false),
                        Attempts = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("clarifier.ModuleStatistics");
        }
    }
}
