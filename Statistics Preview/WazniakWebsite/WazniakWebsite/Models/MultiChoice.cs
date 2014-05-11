using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WazniakWebsite.Models
{
    public class MultiChoice
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [StringLength(200, ErrorMessage = "The Choice is supposed to be reasonably short (up to 200 characters).")]
        public string ChoiceString { get; set; }

        [Required]
        public bool ChoiceBool { get; set; }

        [ForeignKey("MultipleChoiceAnswer")]
        public int MultipleChoiceAnswerID { get; set; }

        public virtual MultipleChoiceAnswer MultipleChoiceAnswer { get; set; }

        public MultiChoice(string choiceString, bool choiceBool)
        {
            ChoiceString = choiceString;
            ChoiceBool = choiceBool;
        }

        public MultiChoice()
        {

        }

        public override string ToString()
        {
            return ChoiceString + " - " + ChoiceBool.ToString().ToUpper();
        }
    }
}