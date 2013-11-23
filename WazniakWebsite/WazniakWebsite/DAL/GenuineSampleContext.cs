using WazniakWebsite.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WazniakWebsite.DAL
{
    public class SchoolContext : DbContext
    {

        public SchoolContext()
            : base("GenuineSampleContext")
        {
        }

        public DbSet<SampleItem> SampleItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<SampleItem>().ToTable("SampleItem", "wazniak_forever");
        }
    }
}