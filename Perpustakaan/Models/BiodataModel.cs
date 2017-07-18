using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perpustakaan.Models
{
    public class BiodataModel
    {
        public int Id { get; set; }
        public int IdState { get; set; }
        public string Username { get; set; }
        public string Alamat { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Makanan { get; set; }
        public string Film { get; set; }
        public DateTime WDaftar { get; set; }
        public string KTP { get; set; }
        public string Foto { get; set; }
    }
}