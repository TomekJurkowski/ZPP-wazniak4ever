using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace WazniakWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This page is yet to be updated. Stay tuned for fresh updates on the work progress!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "If you have any doubts, questions or suggestions on how to make Clarifier better, please don't hesitate to contact us!";
            ViewBag.Message2 = "Here is where you might find us:";
            ViewBag.Message3 = "And here is some other useful contact info:";
            return View();
        }

        private BlobStorageService _blobStorageService = new BlobStorageService();

        public ActionResult Upload()
        {
            var blobContainer = _blobStorageService.GetCloudBLobContainer();
            var blobs = blobContainer.ListBlobs().Select(blobItem => blobItem.Uri.ToString()).ToList();
            return View(blobs);
        }

        /*[HttpPost]
        [ActionName("Upload")]
        public ActionResult UploadPost()
        {
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var image = Request.Files[i];
                if (image == null || image.ContentLength <= 0) continue;
                var blobContainer = _blobStorageService.GetCloudBLobContainer();
                var blob = blobContainer.GetBlockBlobReference(image.FileName);
                blob.UploadFromStream(image.InputStream);
            }
            
            return RedirectToAction("Upload");
        }*/

        private static async Task<Stream> LoadEquationImage(string exp, bool isInline)
        {
            var requestUri = isInline
                ? "http://latex.codecogs.com/png.download?%5Cinline%20" + exp
                : "http://latex.codecogs.com/png.download?%20" + exp;
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var responseMessage = await client.SendAsync(requestMessage);
                return await responseMessage.Content.ReadAsStreamAsync();
            }
        }

        private void UploadImages(IReadOnlyList<Stream> imgStreams, IReadOnlyList<string> fileNames)
        {
            for (var i = 0; i < imgStreams.Count; i++)
            {
                var blobContainer = _blobStorageService.GetCloudBLobContainer();
                var blob = blobContainer.GetBlockBlobReference(fileNames[i]);
                blob.UploadFromStream(imgStreams[i]);
            }
        }

        private async Task UploadFile(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var blobContainer = _blobStorageService.GetCloudBLobContainer();
                var blob = blobContainer.GetBlockBlobReference(file.FileName);
                await blob.UploadFromStreamAsync(file.InputStream);
            }
        }

        [HttpPost]
        [ActionName("Upload")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadPost()
        {
            var text = Request.Form["text"];
            var regex = new Regex("(\\$[^\\$]+?\\$)|(\\$\\$[^\\$]+?\\$\\$)");
            var start = 0;
            var newText = "";
            var i = 0;
            var imgStreams = new List<Stream>();
            var imgNames = new List<string>();
            foreach (Match match in regex.Matches(text))
            {
                Debug.WriteLine("Value: " + match.Value);
                var matchVal = match.Value;
                var notInline = matchVal.StartsWith("$$");
                var imgStream = notInline
                    ? await LoadEquationImage(match.Value.Substring(2, match.Value.Length - 4), false)
                    : await LoadEquationImage(match.Value.Substring(1, match.Value.Length - 2), true);
                imgStreams.Add(imgStream);
                imgNames.Add("Img" + i.ToString(CultureInfo.InvariantCulture));
                newText += text.Substring(start, match.Index - start);

                newText += notInline
                    ? "$$[img:" + i.ToString(CultureInfo.InvariantCulture) + "]$$"
                    : "$[img:" + i.ToString(CultureInfo.InvariantCulture) + "]$";
                start = match.Index + match.Length;
                i++;
            }
            UploadImages(imgStreams, imgNames);
            newText += text.Substring(start, text.Length - start);
            Debug.WriteLine("New text: " + newText);
            return RedirectToAction("Upload");
        }

        [HttpPost]
        public string DeleteImage(string name)
        {
            var uri = new Uri(name);
            var fileName = Path.GetFileName(uri.LocalPath);
            var blobContainer = _blobStorageService.GetCloudBLobContainer();
            var blob = blobContainer.GetBlockBlobReference(fileName);

            blob.Delete();

            return "File Deleted.";
        }

        private async Task<string> EncodeFileFromUrl(string fileName)
        {
            var requestUri = fileName;
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var responseMessage = await client.SendAsync(requestMessage);
                var stream = await responseMessage.Content.ReadAsStreamAsync();
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }

            }

        }

        private string EncodeFileFromFileBase(HttpPostedFileBase file)
        {
                var stream = file.InputStream;
                var fileBytes = new byte[stream.Length];
                stream.Read(fileBytes, 0, (int)stream.Length);
                return Convert.ToBase64String(fileBytes);
        }

        private const string BLOB_URL_PATH =
           "https://clarifierblob.blob.core.windows.net/clarifiermathimages/";

        /*[HttpGet]
        public ActionResult DeleteFile(string id)
        {
            //var uri = new Uri(id, UriKind.RelativeOrAbsolute);
            //var fileName = Path.GetFileName(uri.LocalPath);

            var blockContainer = _blobStorageService.GetCloudBLobContainer();
            var blob = blockContainer.GetBlockBlobReference(id);

            blob.Delete();

            return Json(new {files = new[] {new {id = true}}});
        }

        public async Task<ActionResult> UploadFiles()
        {
            var statuses = new List<ViewDataUploadFilesResult>();
            var encodedFile =
                    await EncodeFileFromUrl("http://www.coffeecup.com/images/icons/applications/web-image-studio_128x128.png");

            JsonResult result;
            if (Request.Files.Count == 0)
            {
                Debug.WriteLine("not upload");
                statuses.Add(new ViewDataUploadFilesResult()
                {
                    name = "lalala",
                    size = 1000,
                    type = "JPG",
                    url = "/Home/Download/",
                    deleteUrl = "/Home/Delete/",
                    thumbnailUrl = @"data:image/png;base64," + encodedFile,
                    //thumbnailUrl = "http://www.coffeecup.com/images/icons/applications/web-image-studio_128x128.png",
                    deleteType = "GET",
                });
                result = Json(new { files = statuses, alreadyUploaded = true });
            }
            else
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    await UploadFile(file);

                    statuses.Add(new ViewDataUploadFilesResult()
                    {
                        name = file.FileName,
                        size = file.ContentLength,
                        type = file.ContentType,
                        url = BLOB_URL_PATH + file.FileName,
                        deleteUrl = "/Home/DeleteFile?id=" + file.FileName,
                        thumbnailUrl = @"data:image/png;base64," + EncodeFileFromFileBase(file),
                        //thumbnailUrl = "http://www.coffeecup.com/images/icons/applications/web-image-studio_128x128.png",
                        deleteType = "GET",
                    });
                }
                result = Json(new { files = statuses, alreadyUploaded = false });
            }


            result.ContentType = "text/plain";
            Debug.WriteLine(result.ToString());
            return result;
        }

        

        public class ViewDataUploadFilesResult
        {
            public string name { get; set; }
            public int size { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public string deleteUrl { get; set; }
            public string thumbnailUrl { get; set; }
            public string deleteType { get; set; }
        }

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/Files")); }
        }*/
    }
}