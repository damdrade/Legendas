using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static Subrip.SubtitlesTime;

namespace Subrip.Controllers
{
    public class UploadFileController : Controller
    {
        public ActionResult Index()
        {
            var items = GetFiles();
            return View(items);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string time, string options)
        {
            Regex pattern = new Regex(@"^\d{1,2}:\d{1,2}:\d{1,2},\d{1,3}$");

            if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).Equals(".srt"))
                if (pattern.Match(time).Success)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/SubRips"), Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        SubtitlesTime.start(path, time, options);
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "Please enter a valid offset";
                }
            else
            {
                ViewBag.Message = "Please select a SubRip file.";
            }

            var items = GetFiles();
            return View(items);
        }

        public FileResult Download(string FileName)
        {
            var FileVirtualPath = "~/SubRips/" + FileName;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }

        private List<string> GetFiles()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/SubRips"));
            System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");

            List<string> items = new List<string>();
            foreach (var file in fileNames)
            {
                items.Add(file.Name);
            }

            return items;
        }

    }
}