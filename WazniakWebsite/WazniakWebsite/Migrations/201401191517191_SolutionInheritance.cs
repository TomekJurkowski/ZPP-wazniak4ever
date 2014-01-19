namespace WazniakWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolutionInheritance : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("wazniak_forever.Solution");
            DropColumn("wazniak_forever.Solution", "SolutionID");
            AddColumn("wazniak_forever.Solution", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("wazniak_forever.Solution", "ID");
            
        }
        
        public override void Down()
        {
            AddColumn("wazniak_forever.Solution", "SolutionID", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("wazniak_forever.Solution");
            AddPrimaryKey("wazniak_forever.Solution", "SolutionID");
            DropColumn("wazniak_forever.Solution", "ID");
        }
    }
}
