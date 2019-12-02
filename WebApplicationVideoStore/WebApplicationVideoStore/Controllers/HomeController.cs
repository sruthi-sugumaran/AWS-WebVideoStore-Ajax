using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationVideoStore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplicationVideoStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<string> LoadComments(string mediaId)
        {
            if (mediaId != null)
            {
                var aws = AWSConnectionService.getInstance();
                var conditions = new List<ScanCondition>();
                conditions.Add(new ScanCondition("GeneratedFileNameAsMediaId", ScanOperator.Equal, mediaId));
                List<MediaComments> commentlist = await aws.context.ScanAsync<MediaComments>(conditions).GetRemainingAsync();
                var json = JsonConvert.SerializeObject(commentlist);
                return json;
                /*
                return Json(new
                {
                    res = commentlist
                });*/
            }
            return null;
        }

        [HttpPost]
        public async Task<string> PostComment(string json)
        {
            dynamic item = JObject.Parse(json);
            string GeneratedFileNameAsMediaId = item.GeneratedFileNameAsMediaId;
            string CommentText = item.CommentText;
            if (GeneratedFileNameAsMediaId!=null && CommentText!=null)
            {
                MediaComments x = new MediaComments(GeneratedFileNameAsMediaId, User.Identity.Name, CommentText);
                await AWSConnectionService.getInstance().context.SaveAsync<MediaComments>(x);
                return JsonConvert.SerializeObject(x);
            }
            return null;
        }

        [HttpPost]
        public async Task<string> LikePost(string json)
        {
            dynamic item = JObject.Parse(json);
            string GeneratedFileNameAsMediaId = item.GeneratedFileNameAsMediaId;
            string CommentText = item.CommentText;
            if (GeneratedFileNameAsMediaId != null && CommentText != null)
            {
                MediaComments x = new MediaComments(GeneratedFileNameAsMediaId, User.Identity.Name, CommentText);
                await AWSConnectionService.getInstance().context.SaveAsync<MediaComments>(x);
                return JsonConvert.SerializeObject(x);
            }
            return null;
        }

        [HttpPost]
        public async Task<string> RateMedia(string json)
        {
            dynamic item = JObject.Parse(json);
            string GeneratedFileNameAsMediaId = item.GeneratedFileNameAsMediaId;
            int RatingValue = item.RatingValue;
            if (GeneratedFileNameAsMediaId != null && RatingValue != 0)
            {
                MediaUserRatings rating = new MediaUserRatings(GeneratedFileNameAsMediaId, User.Identity.Name, RatingValue);
                await AWSConnectionService.getInstance().context.SaveAsync<MediaUserRatings>(rating);
                return JsonConvert.SerializeObject(rating);
            }
            return null;
        }

        [HttpPost]
        public async Task<string> GetMediaRating(string GeneratedFileNameAsMediaId)
        {
            if (GeneratedFileNameAsMediaId != null && User.Identity.IsAuthenticated)
            {
                var rating = await AWSConnectionService.getInstance().context.LoadAsync<MediaUserRatings>(GeneratedFileNameAsMediaId, User.Identity.Name);
                return JsonConvert.SerializeObject(rating);
            }
            return null;
        }

        public async Task<IActionResult> Index()
        {
                ViewData["LoggedIn"] = User.Identity.Name;
                IList<VideoLibrary> videoList = new List<VideoLibrary>();
                var conditions = new List<ScanCondition>();
                videoList = await AWSConnectionService.getInstance().context.ScanAsync<VideoLibrary>(conditions).GetRemainingAsync();
                ViewData["videos"] = videoList;
            return View();
        }




        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<string> Upload(List<IFormFile> files)
        {
            if (files != null)
            {
                IFormFile file = files[0];
                await UploadAsync(file);
                return "uploaded successfully";
            }
            return null;
        }

        private async Task<bool> UploadAsync(IFormFile file)
        {
            var theFile = file;

            // Get the server path, wwwroot
            string webRootPath = _hostingEnvironment.WebRootPath;

            // Building the path to the uploads directory
            var fileRoute = Path.Combine(webRootPath, "uploads");

            // Get the mime type
            var mimeType = file.ContentType;

            // Get File Extension
            string extension = Path.GetExtension(theFile.FileName);

            // Generate Random name.
            string name = Guid.NewGuid().ToString().Substring(0, 8) + extension;

            // Build the full path inclunding the file name
            string link = Path.Combine(fileRoute, name);

            // Create directory if it dose not exist.
            FileInfo dir = new FileInfo(fileRoute);
            dir.Directory.Create();

            // Basic validation on mime types and file extension
            string[] videoMimetypes = { "video/mp4", "video/webm", "video/ogg" };
            string[] videoExt = { ".mp4", ".webm", ".ogg" };

            try
            {
                if (Array.IndexOf(videoMimetypes, mimeType) >= 0 && (Array.IndexOf(videoExt, extension) >= 0))
                {
                    // Copy contents to memory stream.
                    Stream stream;
                    stream = new MemoryStream();
                    theFile.CopyTo(stream);
                    stream.Position = 0;
                    String serverPath = link;

                    await AWSConnectionService.getInstance().uploadFileAndUpdateTable(stream, theFile.FileName, User.Identity.Name);
                    /*
                    // Save the file
                    using (FileStream writerFileStream = System.IO.File.Create(serverPath))
                    {
                       await stream.CopyToAsync(writerFileStream);
                        writerFileStream.Dispose();
                    }*/

                    stream.Dispose();
                    // Return the file path as json
                    Hashtable videoUrl = new Hashtable();
                    videoUrl.Add("link", "/uploads/" + name);
                    return true;
                }
                throw new ArgumentException("The video did not pass the validation");
                
            }

            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }
    }
}
