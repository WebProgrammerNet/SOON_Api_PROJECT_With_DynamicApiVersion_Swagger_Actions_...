using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.DataTransferObjects
{
    public class URLDto
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }

        public URLDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
