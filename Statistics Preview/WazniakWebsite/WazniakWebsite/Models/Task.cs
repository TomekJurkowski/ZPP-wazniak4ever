using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class Task
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public int SubjectID { get; set; }
        public int CorrectAnswers { get; set; }
        public int Attempts { get; set; }
        
        public virtual Subject Subject { get; set; }
        
        public virtual Answer Answer { get; set; }

        public Task(string title)
        {
            Title = title;
        }

        public Task()
        {

        }

        public virtual string className()
        {
            return "Task";
        }

    }
}