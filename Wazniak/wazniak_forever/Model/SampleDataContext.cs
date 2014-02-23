using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wazniak_forever.Model
{
    public class SampleItem
    {
        public int Id { get; set; }

        public string Text { get; set; }
        
        public int? Number { get; set; }

    }

    public class SampleItemContext 
    {
        //public IMobileServiceTable<SampleItem> Items = App.MobileService.GetTable<SampleItem>();
    }
}
