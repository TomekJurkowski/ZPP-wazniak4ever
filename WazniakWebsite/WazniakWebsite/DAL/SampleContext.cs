using Microsoft.WindowsAzure.MobileServices;
using WazniakWebsite.Models;

namespace WazniakWebsite.DAL
{
    public class SampleContext
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://wazniak-forever.azure-mobile.net/",
            "uaychyZiwJPIdMZaiMsYPhMfcQXDCN36"
            );

        public IMobileServiceTable<SampleItem> SampleItems
        {
            get
            {
                return MobileService.GetTable<SampleItem>();
            }
        }
    }

}