namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextAnswer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("wazniak_forever.Solution", "correctAnswer", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("wazniak_forever.Solution", "correctAnswer", c => c.Int());
        }
    }
}
