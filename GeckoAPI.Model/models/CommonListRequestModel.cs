using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class CommonListRequestModel
    {
        public long PageNumber { get; set; }
        public long PageSize { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }
}
