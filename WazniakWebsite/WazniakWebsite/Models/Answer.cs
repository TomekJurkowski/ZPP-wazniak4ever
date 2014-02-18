using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WazniakWebsite.Models
{
    public class Answer
    {
        [Key]
        [ForeignKey("Task")]
        public int TaskID { get; set; }

        public virtual Task Task { get; set; }
    }
}