using System.ComponentModel.DataAnnotations;


namespace WazniakWebsite.Models
{
    public class MathematicalTask
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Text { get; set; }
    }
}