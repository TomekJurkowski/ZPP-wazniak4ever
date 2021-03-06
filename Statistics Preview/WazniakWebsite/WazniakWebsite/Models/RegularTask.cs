﻿
using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class RegularTask : Task
    {
        [Required]
        public string Text { get; set; }

        public RegularTask(string text, string title) : base(title)
        {
            Text = text;
        }

        public RegularTask()
        {

        }

        public override string className()
        {
            return "RegularTask";
        }
    }
}