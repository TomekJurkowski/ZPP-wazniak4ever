
using System.ComponentModel.DataAnnotations;

namespace WazniakWebsite.Models
{
    public class MathematicalTask : Task
    {
        // In this string we will hold the actual mathematical task content, that the user
        // has created in the Create page - it will still have mathematical formulas surrounded
        // by $ signs.
        [Required]
        public string Text { get; set; }

        // In this attribute we will hold a slightly modified content provided by the user.
        // Mathematical formulas surrounded by $ signs will be replaced by a name of an image
        // that has been made during creation of a task.

        public string ModifiedText { get; set; }

        public override string className()
        {
            return "MathematicalTask";
        }
    }
}