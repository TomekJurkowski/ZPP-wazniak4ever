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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Subject>().ToTable("Subject", "clarifier");
            modelBuilder.Entity<Task>().ToTable("Task", "clarifier");
            modelBuilder.Entity<Answer>().ToTable("Answer", "clarifier");
        }

        public DbSet<MathematicalTask> MathematicalTasks { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<AbcdAnswer> AbcdAnswers { get; set; }
        public DbSet<TextAnswer> TextAnswers { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<RegularTask> RegularTasks { get; set; }
        public DbSet<SingleValueAnswer> SingleValueAnswers { get; set; }
        public DbSet<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }
    }
}