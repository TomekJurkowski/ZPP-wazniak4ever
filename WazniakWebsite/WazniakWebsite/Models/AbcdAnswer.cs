using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WazniakWebsite.Models
{
    public class AbcdAnswer : Solution
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
    }
}