namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextAnswer1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("wazniak_forever.Solution", "correctAnswer", c => c.Int());
            AlterColumn("wazniak_forever.Solution", "CorrectAnswer", c => c.Int());
            AlterColumn("wazniak_forever.Solution", "correctAnswer", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("wazniak_forever.Solution", "correctAnswer", c => c.String());
            AlterColumn("wazniak_forever.Solution", "CorrectAnswer", c => c.String());
            AlterColumn("wazniak_forever.Solution", "correctAnswer", c => c.Int());
        }
    }
}
