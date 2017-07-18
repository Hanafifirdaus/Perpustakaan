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
        public ActionResult DaftarBuku(string sortOrder, string currentFilter, string searchString, int? page)
        {
            Session["Admin"] = "Root";
            Session["User"] = 1;

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
            //int j = int.Parse(Session["User"].ToString());

            DBukuModel mode = context.DBukus.Where(model => model.Id == id).Select(model => new DBukuModel()
            {
                Judul = model.Judul,
                Penerbit = model.Penerbit,
                LTerbit = model.LTerbit,
                Pengarang = model.Pengarang,
                Jumlah = model.Jumlah,
                IdBuku = (int)model.IdBuku,
                Images = model.Images,
                Id = model.Id,
            }).SingleOrDefault();

            int ka = (int)Session["KTP"];


            DateTime now = DateTime.Now;
            Pinjam p = new Pinjam()
            {
                WPinjam = DateTime.Today,
                IdUser = (int)Session["KTP"],
                PBuku = mode.Judul,
                IdBuku = mode.Id
            };

            context.Pinjams.InsertOnSubmit(p);
            context.SubmitChanges();

            return RedirectToAction("DaftarBuku");
        }

        public ActionResult SignUp()
        {

            BiodataModel mode = new BiodataModel();
            return View(mode);
        }

        [HttpPost]
        public ActionResult SignUp(BiodataModel biodata, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {

                var SeachData = context.Biodatas.Where(x => x.Pass == biodata.Username).SingleOrDefault();
                if (SeachData != null)
                {
                    ModelState.AddModelError(string.Empty, "Student Name already exists.");
                    return View();
                }
            }

            string imageUrl = "";
            if (file != null)
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string physicalPath = Server.MapPath("~/Profil/" + ImageName);
                file.SaveAs(physicalPath);

                imageUrl = ImageName;
            }

            try
            {
                Biodata Bio = new Biodata()
                {
                    Username = biodata.Username,
                    Alamat = biodata.Alamat,
                    Email = biodata.Email,
                    Pass = biodata.Pass,
                    Makanan = biodata.Makanan,
                    Film = biodata.Film,
                    WDaftar = DateTime.Now,
                    IdStat = 2,
                    Foto = imageUrl,
                    KTP = biodata.KTP
                };

                context.Biodatas.InsertOnSubmit(Bio);
                context.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

            
        }

        public ActionResult Profil(int Id)
        {
            List<TBacaModel> Baca = new List<TBacaModel>();
            var querys = from Biodata in context.Biodatas
                         join TBaca in context.TBacas on Biodata.Id equals TBaca.IdUser where TBaca.IdUser == Id
                         select new TBacaModel
                         {
                             Id = TBaca.Id,
                             User = Biodata.Username,
                             Buku = TBaca.Buku,
                             IdUser = TBaca.IdUser,
                             ReWaktu = (DateTime)TBaca.ReWaktu
                         };

            List<BiodataModel> Bio = new List<BiodataModel>();
            var query = from Biodata in context.Biodatas where Biodata.Id == Id
                         select new BiodataModel
                         {
                             Id = Biodata.Id,
                             Username = Biodata.Username,
                             Alamat = Biodata.Alamat,
                             Email = Biodata.Email,
                             Pass = Biodata.Pass,
                             WDaftar = (DateTime)Biodata.WDaftar,
                             KTP = Biodata.KTP,
                             Foto = Biodata.Foto
                         };

            //List<TBacaModel> TB = new List<TBacaModel>();

            var tupleModel = new Tuple<List<TBacaModel>, List<BiodataModel>>(querys.ToList(), query.ToList());
            return View(tupleModel);
            //tupleModel
        }

        
        public ActionResult Create(string searchString)
        {
            TBaca buku = new TBaca()
            {
                Buku = searchString,
                ReWaktu = DateTime.Now,
                IdUser = (int)Session["KTP"]
            };

            context.TBacas.InsertOnSubmit(buku);
            context.SubmitChanges();
            return RedirectToAction("Profil/"+(int)Session["KTP"]);
        }

        public ActionResult Index()
        {
            BiodataModel model = new BiodataModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(BiodataModel login)
        {
            var query = from p in context.Biodatas
                    where p.Username == login.Username
                    && p.Pass == login.Pass
                    select p;

            if (query.Any())
            {
                string KTP = "";
                int Id = 0;
                foreach(var a in query)
                {
                    KTP = a.KTP;
                    Id = a.Id;
                }
                Session["KTP"] = Id;
                return RedirectToAction("Profil/"+Id);
            }
            else
                return View("Index");
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

        public ActionResult hase()
        {
            return View();
        }
    }
}