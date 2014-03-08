using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class SingleValueAnswer : Answer
    {
        [Required]
        [StringLength(100, ErrorMessage = "The SingleValueAnswer is supposed to be short (up to 100 characters).\n" +
                                          "For a longer answer consider using TextAnswer.")]
        public string Value { get; set; }

        public SingleValueAnswer(string value)
        {
            Value = value;
        }

        public SingleValueAnswer()
        {
            
        }

        public override string Overview()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override string className()
        {
            return SINGLE_VALUE_ANSWER;
        }
    }
}