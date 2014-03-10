﻿using System;
using System.Collections.Generic;
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

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy, HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Update Date")]
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

        public void UpdateLastUpdatedTime()
        {
            LastUpdated = DateTime.Now;
        }
    }
}