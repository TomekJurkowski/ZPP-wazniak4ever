using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImagesUpload
{
    public class BlobStorageService
    {
        public CloudBlobContainer GetCloudBLobContainer()
        {
            var storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("ImageStorageConn"));
            var blobCLient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobCLient.GetContainerReference("myimages");
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