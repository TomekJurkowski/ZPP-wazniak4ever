using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wazniak_forever.Model
{
    public class Course
    {
        public string Name { get; private set; }

        public Course(string Name)
        {
            this.Name = Name;
        }
    }
}
