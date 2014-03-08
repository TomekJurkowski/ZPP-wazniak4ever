
using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class SingleChoice
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The Choice is supposed to be reasonably short (up to 200 characters).")]
        public string ChoiceString { get; set; }

        public int SingleChoiceAnswerID { get; set; }

        public virtual SingleChoiceAnswer SingleChoiceAnswer { get; set; }
    }
}