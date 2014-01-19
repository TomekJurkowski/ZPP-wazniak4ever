namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextAnswer2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("wazniak_forever.Solution", "answer", c => c.String());
            AlterColumn("wazniak_forever.Solution", "CorrectAnswer", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("wazniak_forever.Solution", "CorrectAnswer", c => c.Int());
            DropColumn("wazniak_forever.Solution", "answer");
        }
    }
}
