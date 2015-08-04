using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Devq.ErrorLog.Models;
using Devq.ErrorLog.ViewModels;

namespace Devq.ErrorLog.Controllers
{
    public class AdminController : Controller {
        
        private string _basePath = string.Empty;
        private readonly string[] _types = {"error", "debug"};

        public ActionResult Index(string selectedLogFileName, string selectedType)
        {
            _basePath = Server.MapPath(@"~/App_Data/Logs");
            var model = new IndexViewModel();
            var type = string.IsNullOrEmpty(selectedType) ? _types[0] : selectedType;

            string logFileName;

            if (string.IsNullOrEmpty(selectedLogFileName) || GetType(selectedLogFileName) != selectedType) {
                logFileName = string.Format("orchard-{0}-{1}.{2}.{3}.log",
                        type,
                        DateTime.Now.Year,
                        DateTime.Now.Month.ToString().PadLeft(2, '0'),
                        DateTime.Now.Day.ToString().PadLeft(2, '0'));
            }
            else {
                logFileName = selectedLogFileName;
            }

            var fullLogFilePath = _basePath + "/" + logFileName;
            model.LogDate = logFileName;

            return View(GetModel(fullLogFilePath, model, type));
        }

        private IndexViewModel GetModel(string fileName, IndexViewModel model, string type) {

            var logs = Directory
                .GetFiles(_basePath, "*.log")
                .Where(l => l.IndexOf(string.Format("orchard-{0}-", type), StringComparison.Ordinal) >= 0)
                .Select(l => l.Substring(l.LastIndexOf("\\", StringComparison.Ordinal) + 1))
                .OrderByDescending(l => l);
            
            var dates = logs.Select(logFile => new SelectListItem
            {
                Text = logFile,
                Value = logFile
            });

            var types = _types.Select(t => new SelectListItem {
                Text = t,
                Value = t
            });

            model.Dates = dates;
            model.Types = types;
            
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    model.LogText = reader.ReadToEnd();
                    var regex = new Regex(@"([0-9]{4}-[0-9]{2}-[0-9]{2}\s[0-9]{2}:[0-9]{2}:[0-9]{2})");

                    var matches = regex.Split(model.LogText).Where(s => s != String.Empty).ToArray();

                    for (var i = 0; i < matches.Count(); i++)
                    {
                        model.LogItems.Insert(0, new LogItem
                        {
                            Date = matches[i],
                            Text = matches[i + 1]
                        });

                        i++;
                    }
                }
            }

            return model;
        }

        private string GetType(string file) {
            foreach (var type in _types) {
                if (file.IndexOf(type) > -1) {
                    return type;
                }
            }

            return _types[0];
        }
    }
}