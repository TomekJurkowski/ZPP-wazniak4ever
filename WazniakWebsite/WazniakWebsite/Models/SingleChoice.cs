
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WazniakWebsite.Models
{
    public class SingleChoice
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The Choice is supposed to be reasonably short (up to 200 characters).")]
        public string ChoiceString { get; set; }

        [ForeignKey("SingleChoiceAnswer")]
        public int SingleChoiceAnswerID { get; set; }

        public virtual SingleChoiceAnswer SingleChoiceAnswer { get; set; }

        public SingleChoice(string choiceString)
        {
            ChoiceString = choiceString;
        }

        public SingleChoice()
        {

        }

        public override string ToString()
        {
            return ChoiceString;
        }
    }
}