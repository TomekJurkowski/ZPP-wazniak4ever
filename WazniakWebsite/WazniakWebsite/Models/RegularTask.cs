using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class RegularTask : Task
    {
        public string Text { get; set; }
    }
}