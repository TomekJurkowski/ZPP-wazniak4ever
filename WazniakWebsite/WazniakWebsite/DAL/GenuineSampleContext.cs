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
            modelBuilder.Entity<Subject>().ToTable("Subject", "wazniak_forever");
            modelBuilder.Entity<MathematicalTask>().ToTable("MathematicalTask", "wazniak_forever");
            modelBuilder.Entity<Solution>().ToTable("Solution", "wazniak_forever");
        }

        public DbSet<MathematicalTask> MathematicalTasks { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<AbcdAnswer> AbcdAnswers { get; set; }
        public DbSet<TextAnswer> TextAnswers { get; set; }
        public DbSet<Solution> Solutions { get; set; }
    }
}