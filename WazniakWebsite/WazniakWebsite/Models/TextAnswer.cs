using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class TextAnswer : Answer
    {
        [Required]
        public string Text { get; set; }
    }
}