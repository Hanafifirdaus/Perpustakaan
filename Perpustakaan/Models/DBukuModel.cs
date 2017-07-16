using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Perpustakaan.Models
{
    
    public class DBukuModel
    {
        public int Id { get; set; }
        public string Judul { get; set; }
        public string Penerbit { get; set; }
        public string LTerbit { get; set; }
        public string Pengarang { get; set; }
        public int Jumlah { get; set; }
        public int IdBuku { get; set; }
        public string Images { get; set; }

        public string GenreBuku { get; set; }
        public int GenreId { get; set; }

        public IEnumerable<SelectListItem> Kelompoks { get; set; }

        public DBukuModel()
        {
            Kelompoks = new List<SelectListItem>();
        }
    }
}