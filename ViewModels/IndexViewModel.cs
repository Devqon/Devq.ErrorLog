using System.Collections.Generic;
using System.Web.Mvc;
using Laughlin.ErrorLog.Models;

namespace Laughlin.ErrorLog.ViewModels
{
    public class IndexViewModel
    {
        public string FileName { get; set; }
        public string LogText { get; set; }
        public string LogDate { get; set; }
        public List<LogItem> LogItems { get; set; }
        
        public IEnumerable<SelectListItem> Dates { get; set; }
        public string SelectedDate { get; set; }

        public IndexViewModel() {
            LogItems = new List<LogItem>();
        }
    }
}