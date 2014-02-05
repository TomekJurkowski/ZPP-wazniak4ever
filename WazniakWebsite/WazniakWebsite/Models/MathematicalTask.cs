﻿using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class MathematicalTask : Solution
    {
        [StringLength(50)]
        public string Title { get; set; }

        public string Text { get; set; }
    }
}