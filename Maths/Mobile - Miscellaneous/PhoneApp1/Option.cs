using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PhoneApp1
{
    public class Option
    {
        public string Name { get; private set; }
        public string ImageUrl { get; private set; }

        public Option(string Name, string ImageUrl)
        {
            this.Name = Name;
            this.ImageUrl = ImageUrl;
        }
    }
}
