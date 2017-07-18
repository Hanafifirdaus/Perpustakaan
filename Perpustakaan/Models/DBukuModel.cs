using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Perpustakaan.Models
{
    
    public class DBukuModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter student name.")]
        public string Judul { get; set; }
        [Required(ErrorMessage = "Please enter student name.")]
        public string Penerbit { get; set; }
        [Required(ErrorMessage = "Please enter student name.")]
        public string LTerbit { get; set; }
        [Required(ErrorMessage = "Please enter student name.")]
        public string Pengarang { get; set; }
        [Required(ErrorMessage = "Please enter student name.")]
        public int Jumlah { get; set; }
        [Required]
        public int IdBuku { get; set; }
        public string Images { get; set; }
        [Required]
        public string GenreBuku { get; set; }
        public int GenreId { get; set; }

        public IEnumerable<SelectListItem> Kelompoks { get; set; }

        public DBukuModel()
        {
            Kelompoks = new List<SelectListItem>();
        }
    }
}