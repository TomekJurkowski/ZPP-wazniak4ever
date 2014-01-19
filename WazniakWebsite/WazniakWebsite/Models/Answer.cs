using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WazniakWebsite.Models
{
    public class Answer
    {
        public int ID { get; set; }
        public string content { get; set; }
    }
}