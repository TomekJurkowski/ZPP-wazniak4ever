
using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class TrueFalseABCAnswer : Solution
    {
        [Required]
        public string StatementA { get; set; }

        [Required]
        public string StatementB { get; set; }

        [Required]
        public string StatementC { get; set; }

        [Required]
        public bool AnswerA { get; set; }

        [Required]
        public string AnswerB { get; set; }

        [Required]
        public string AnswerC { get; set; }
    }
}