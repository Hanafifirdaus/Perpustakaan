using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perpustakaan.Models
{
    public class TBacaModel
    {
        public int Id { get; set; }
        public string Buku { get; set; }
        public int IdGenre { get; set; }
        public DateTime ReWaktu { get; set; }
        public int IdUser { get; set; }

        public string User { get; set; }
    }
}