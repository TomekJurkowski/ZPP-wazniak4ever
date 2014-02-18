using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class Task
    {
        [Key]
        public int ID { get; set; }

        public int SubjectID { get; set; }
        
        public int AnswerID { get; set; }

        public virtual Subject Subject { get; set; }
        
        public virtual Answer Answer { get; set; }

    }
}