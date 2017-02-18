using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Data
{
    public class DownloadItem
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }
        public string Html { get; set; }
        public DownloadStatus Status { get; set; }
    }
}
