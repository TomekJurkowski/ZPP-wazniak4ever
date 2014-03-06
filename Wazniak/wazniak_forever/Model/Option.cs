using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wazniak_forever.Model
{
    public enum OptionType
    {
        MyCourses, Downloads,
        Settings, Start, StudyWithClarifier, AddToMyCourses, Download,
        Login, Logout, Update, DeleteFromMyCourses,
        DeleteFromDownloads
    }

    public class Option
    {
        public OptionType Type { get; private set; }
        public bool OnlineOnly { get; private set; }
        public string Name { get; private set; }
        public Uri ImageUrl { get; private set; }

        public Option(OptionType Type, bool OnlineOnly, string Name, Uri ImageUrl)
        {
            this.Type = Type;
            this.OnlineOnly = OnlineOnly;
            this.Name = Name;
            this.ImageUrl = ImageUrl;
        }
    }
}
