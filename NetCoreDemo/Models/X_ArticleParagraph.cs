using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDemo.Models
{
    public class X_ArticleParagraph
    {
        public string ID { get; set; }
        public string AID { get; set; }
        public string Type { get; set; }
        public int GLevel { get; set; }
        public string Fid { get; set; }
        public int OrderBy { get; set; }
        public string ParagraphContent { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public string CreateIP { get; set; }
        public DateTime CreateTime { get; set; }
        public string LastUpdater { get; set; }
        public string LastUpdateIP { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
