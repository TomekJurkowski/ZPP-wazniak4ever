using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class Subject
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "You have to specify the Name of the Subject")]
        [StringLength(50)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}