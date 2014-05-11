using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public ActionResult Statistics()
        {
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
                System.Diagnostics.Debug.WriteLine("Value: " + match.Value);
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
            System.Diagnostics.Debug.WriteLine("New text: " + newText);
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
    }
}