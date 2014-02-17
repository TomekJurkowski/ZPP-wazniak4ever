using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class SingleValueAnswer : Solution
    {
        [Required]
        public string Text { get; set; }
    }
}