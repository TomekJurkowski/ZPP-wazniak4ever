using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wazniak_forever.Model
{
    public enum OptionType { MyCourses, Downloads, Settings }

    public class Option
    {
        public OptionType Type { get; private set; }
        public string Name { get; private set; }
        public Uri ImageUrl { get; private set; }

        public Option(OptionType Type, string Name, Uri ImageUrl)
        {
            this.Type = Type;
            this.Name = Name;
            this.ImageUrl = ImageUrl;
        }
    }
}
