﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WazniakWebsite.Models
{
    public class TaskStatistics
    {
        [Key]
        public int ID { get; set; }
        public int TaskID { get; set; }
        public int CorrectAnswers { get; set; }
        public int Attempts { get; set; }
    }
}