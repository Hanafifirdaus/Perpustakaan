using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perpustakaan.Models
{
    public class SPinjamModel
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string PBuku { get; set; }
        public DateTime WKembali { get; set; }
        public string User { get; set; }
        public int IdBuku { get; set; }
    }
}