﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class AbcdAnswer : Answer
    {
        [Required]
        public string A { get; set; }

        [Required]
        public string B { get; set; }

        [Required]
        public string C { get; set; }

        [Required]
        public string D { get; set; }
        
        [DisplayName("Correct Answer")]
        [Required(ErrorMessage = "You have to specify the correct answer")]
        [Range(0, 3, ErrorMessage = "The field 'Correct Answer' must be between 0 and 3, where 0 stands for A and 3 stands for D")]
        public int CorrectAnswer { get; set; }

        public override string Overview()
        {
            return "SingleChoiceAnswer with the " + CorrectAnswer + " option correct.";
        }

        public override string ToString()
        {
            return "SingleChoiceAnswer with the " + CorrectAnswer + " option correct.";
        }

        public override string className()
        {
            return SINGLE_CHOICE_ANSWER;
        }
    }
}