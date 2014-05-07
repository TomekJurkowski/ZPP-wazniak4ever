using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WazniakWebsite
{
    public class BlobStorageService
    {
        public CloudBlobContainer GetCloudBLobContainer()
        {
            //var storageAccount =
            //    CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("ClarifierBlobConnectionString"));
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("ClarifierBlobConnectionString"));

            var blobCLient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobCLient.GetContainerReference("clarifiermathimages");
            if (blobContainer.CreateIfNotExists())
            {
                blobContainer.SetPermissions((new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                }));
            }
            return blobContainer;
        }
    }
}