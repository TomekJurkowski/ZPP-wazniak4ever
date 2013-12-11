using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class Subject
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}