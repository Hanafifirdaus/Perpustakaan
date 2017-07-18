using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Perpustakaan.Models
{
    public class PinjamModel
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string PBuku { get; set; }
        public DateTime WPinjam { get; set; }
        public string User { get; set; }
        public bool Confirm { get; set; }
        public int IdBuku { get; set; }
        public string Status { get; set; }

        public IEnumerable<SelectListItem> biodatas { get; set; }

        public PinjamModel()
        {
            biodatas = new List<SelectListItem>();
        }
    }
}