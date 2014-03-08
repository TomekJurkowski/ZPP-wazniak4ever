using System.ComponentModel.DataAnnotations;

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

        public int MultipleChoiceAnswerID { get; set; }

        public virtual MultipleChoiceAnswer MultipleChoiceAnswer { get; set; }

        public override string ToString()
        {
            return ChoiceString + "\t" + ChoiceBool;
        }
    }
}