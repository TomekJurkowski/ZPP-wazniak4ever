namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTaskStatisticsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "clarifier.TaskStatistics",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        CorrectAnswers = c.Int(nullable: false),
                        Attempts = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("clarifier.TaskStatistics");
        }
    }
}
