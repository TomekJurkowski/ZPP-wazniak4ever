using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class Module
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public int SequenceNo { get; set; }

        public int SubjectID { get; set; }

        public virtual Subject Subject { get; set; }
    }
}