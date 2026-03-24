using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class ContactUs
    {
        public long? ContactUsId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? ContactSubject { get; set; }
        public string? CustomerMessage { get; set; }
        public string? AdminMessage { get; set; }
        public bool? IsReplied { get; set; }
        public long? TotalRecords { get; set; }
    }
}
