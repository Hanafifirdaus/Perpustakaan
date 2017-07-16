using PagedList;
using Perpustakaan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Perpustakaan.Controllers
{
    public class HomeController : Controller
    {
        private OperationDataContext context = null;
        public HomeController()
        {
            context = new OperationDataContext();
        }
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            List<AutenModel> aut = new List<AutenModel>();
            var secret = from sec in context.Autens select new AutenModel { Username = sec.Username, AutSes = sec.AutSes, Id = sec.Id };
            aut = secret.ToList();
            var sessi = from se in aut where se.Id == aut.Count select new { se.Username, se.AutSes };
            string a = "";
            string b = "";
            foreach (var open in sessi)
            {
                a = open.Username;
                b = open.AutSes;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Judul" : "";
            ViewBag.DateSortParm = sortOrder == "Pengarang" ? "Judul" : "Pengarang";
            var students = from s in context.DBukus
                           select s;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var query = from book in context.DBukus
                        join Kel in context.Kelompoks on book.IdBuku equals Kel.Id
                        select new DBukuModel
                        {
                            Id = book.Id,
                            Judul = book.Judul,
                            Penerbit = book.Penerbit,
                            LTerbit = book.LTerbit,
                            Pengarang = book.Pengarang,
                            Jumlah = book.Jumlah,
                            GenreBuku = Kel.GBuku,
                            Images = book.Images
                        };

            if (!String.IsNullOrEmpty(searchString))
            {

                query = query.Where(s => s.Judul.Contains(searchString) || s.Penerbit.Contains(searchString) ||
                s.GenreBuku.Contains(searchString) || s.Pengarang.Contains(searchString) || s.LTerbit.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Judul":
                    query = query.OrderByDescending(s => s.Judul);
                    break;
                case "Pengarang":
                    query = query.OrderBy(s => s.Pengarang);
                    break;
                case "Penerbit":
                    query = query.OrderByDescending(s => s.Penerbit);
                    break;
                default:
                    query = query.OrderBy(s => s.GenreBuku);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(query.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Pinjam(int id)
        {
            DBukuModel mode = context.DBukus.Where(model => model.Id == id).Select(model => new DBukuModel()
            {
                Judul = model.Judul,
                Penerbit = model.Penerbit,
                LTerbit = model.LTerbit,
                Pengarang = model.Pengarang,
                Jumlah = model.Jumlah,
                IdBuku = (int)model.IdBuku,
                Images = model.Images,
                Id = model.Id
            }).SingleOrDefault();
            

            DateTime now = DateTime.Now;
            Pinjam p = new Pinjam()
            {
                WPinjam = DateTime.Today,
                IdUser = 4,
                PBuku = mode.Judul,
                IdBuku = mode.Id
            };

            context.Pinjams.InsertOnSubmit(p);
            context.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}