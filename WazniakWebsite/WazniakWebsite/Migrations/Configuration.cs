namespace WazniakWebsite.Migrations
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.SchoolContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WazniakWebsite.DAL.SchoolContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var sampleItems = new List<SampleItem>()
            {
                new SampleItem { Text = "text1", Number = 10 },
                new SampleItem { Text = "text2", Number = 20 }
            };
            sampleItems.ForEach(s => context.SampleItems.AddOrUpdate(s));
            context.SaveChanges();
        }
    }
}
