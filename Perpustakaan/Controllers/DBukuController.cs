using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Perpustakaan.Models;
using System.Security.Cryptography;
using System.Text;
using PagedList.Mvc;
using PagedList;

namespace Perpustakaan.Controllers
{
    public class DBukuController : Controller
    {
        private OperationDataContext context = null;

        public object MyProductDataSource { get; private set; }
        public object ShoppingCart { get; private set; }

        public DBukuController()
        {
            context = new OperationDataContext();
        }

        // GET: DBuku
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            List<AutenModel> aut = new List<AutenModel>();
            var secret = from sec in context.Autens select new AutenModel { Username = sec.Username, AutSes = sec.AutSes};
            aut = secret.ToList();
            int k = aut.Count();
            string a = "";
            string b = "";
            foreach (var open in aut)
            {
                a = open.Username;
                b = open.AutSes;
            }

            if (Session[a] == null)
            {
                return RedirectToAction("SignIn");
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
            return View(query.OrderByDescending(s => s.Id).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            List<AutenModel> aut = new List<AutenModel>();
            var secret = from sec in context.Autens select new AutenModel { Username = sec.Username, AutSes = sec.AutSes };
            aut = secret.ToList();
            int k = aut.Count();
            string a = "";
            string b = "";
            foreach (var open in aut)
            {
                a = open.Username;
                b = open.AutSes;
            }

            if (Session[a] == null)
            {
                return RedirectToAction("SignIn");
            }

            DBukuModel model = new DBukuModel();
            PreparePublisher(model);

            return View(model);
        }

        private void PreparePublisher(DBukuModel model)
        {
            model.Kelompoks = context.Kelompoks.AsQueryable<Kelompok>().Select(x =>
            new SelectListItem()
            {
                Text = x.GBuku,
                Value = x.Id.ToString(),
            });
        }

        [HttpPost]
        public ActionResult Create(DBukuModel model, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            string imageUrl = "";
            if (file != null)
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string physicalPath = Server.MapPath("~/images/" + ImageName);
                file.SaveAs(physicalPath);

                imageUrl = ImageName;
            }
            try
            {
                DBuku buku = new DBuku()
                {
                    Judul = model.Judul,
                    Penerbit = model.Penerbit,
                    LTerbit = model.LTerbit,
                    Pengarang = model.Pengarang,
                    Jumlah = model.Jumlah,
                    IdBuku = model.IdBuku,
                    Images = imageUrl,
                    Id = model.Id
                };

                context.DBukus.InsertOnSubmit(buku);
                context.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        public ActionResult Edit(int? id)
        {
            List<AutenModel> aut = new List<AutenModel>();
            var secret = from sec in context.Autens select new AutenModel { Username = sec.Username, AutSes = sec.AutSes };
            aut = secret.ToList();
            int k = aut.Count();
            string a = "";
            string b = "";
            foreach (var open in aut)
            {
                a = open.Username;
                b = open.AutSes;
            }

            if (Session[a] == null)
            {
                return RedirectToAction("SignIn");
            }

            if (id == null)
            {
                return HttpNotFound();
            }

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
            

            PreparePublisher(mode);
            return View(mode);
        }

        [HttpPost]
        public ActionResult Edit(DBukuModel model, HttpPostedFileBase file)
        {
            string imageUrl = "";
            if (file != null)
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string physicalPath = Server.MapPath("~/images/" + ImageName);
                file.SaveAs(physicalPath);

                imageUrl = ImageName;
            }
            try
            {
                DBuku buku = context.DBukus.Where(some => some.Id == model.Id).Single<DBuku>();
                buku.Judul = model.Judul;
                buku.Penerbit = model.Penerbit;
                buku.LTerbit = model.LTerbit;
                buku.Pengarang = model.Pengarang;
                buku.Jumlah = model.Jumlah;
                buku.IdBuku = (int)model.IdBuku;
                buku.Images = imageUrl;
                buku.Id = model.Id;
                 
                context.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult SignIn()
        {
            BiodataModel model = new BiodataModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult SignIn(BiodataModel login)
        {
            var q = from p in context.Biodatas
                    where p.Username == login.Username
                    && p.Pass == login.Pass && p.IdStat == 1
                    select p;
            
            if (q.Any())
            {
                DateTime now = DateTime.Now;
                Auten auten = new Auten();
                {
                    auten.Username = login.Username;
                    auten.AutSes = MD5Hash(login.Username + login.Pass + now);
                };

                context.Autens.InsertOnSubmit(auten);
                context.SubmitChanges();
                
                Session[login.Username] = MD5Hash(login.Username + login.Pass + now);
                return RedirectToAction("Index");
            }
            else
            {
                return View("SignIn");
            }
        }

        public ActionResult Delete(int id)
        {
            List<AutenModel> aut = new List<AutenModel>();
            var secret = from sec in context.Autens select new AutenModel { Username = sec.Username, AutSes = sec.AutSes };
            aut = secret.ToList();
            int k = aut.Count();
            string a = "";
            string b = "";
            foreach (var open in aut)
            {
                a = open.Username;
                b = open.AutSes;
            }

            if (Session[a] == null)
            {
                return RedirectToAction("SignIn");
            }

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

            PreparePublisher(mode);
            return View(mode);
        }

        [HttpPost]
        public ActionResult Delete(DBukuModel model)
        {
            try
            {
                DBuku buku = context.DBukus.Where(some => some.Id == model.Id).Single<DBuku>();
                buku.Id = model.Id;

                context.DBukus.DeleteOnSubmit(buku);
                context.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Confirm()
        {
            List<AutenModel> aut = new List<AutenModel>();
            var secret = from sec in context.Autens select new AutenModel { Username = sec.Username, AutSes = sec.AutSes };
            aut = secret.ToList();
            int k = aut.Count();
            string a = "";
            string b = "";
            foreach (var open in aut)
            {
                a = open.Username;
                b = open.AutSes;
            }

            if (Session[a] == null)
            {
                return RedirectToAction("SignIn");
            }

            List<SPinjamModel> spinjams = new List<SPinjamModel>();
            var querys = from Biodata in context.Biodatas
                        join SPinjam in context.SPinjams on Biodata.Id equals SPinjam.IdUser
                        select new SPinjamModel
                        {
                            Id = SPinjam.Id,
                            User = Biodata.Username,
                            IdUser = SPinjam.IdUser,
                            PBuku = SPinjam.PBuku,
                            WKembali = (DateTime)SPinjam.WKembali,
                            IdBuku = (int)SPinjam.IdBuku,
                            Status = SPinjam.Status
                        };
            spinjams = querys.ToList();


            List<PinjamModel> pinjams = new List<PinjamModel>();
            var query = from Biodata in context.Biodatas
                        join Pinjam in context.Pinjams on Biodata.Id equals Pinjam.IdUser
                        select new PinjamModel
                        {
                            User = Biodata.Username,
                            PBuku = Pinjam.PBuku,
                            WPinjam = DateTime.Now,
                            IdUser = Pinjam.IdUser,
                            IdBuku = (int)Pinjam.IdBuku,
                            Id = Pinjam.Id
                        };
            pinjams = query.ToList();

            var tupleModel = new Tuple<List<PinjamModel>, List<SPinjamModel>>(query.ToList(), querys.ToList());
            return View(tupleModel);
        }

        public ActionResult Acc(int id)
        {
            List<PinjamModel> SP = new List<PinjamModel>();
            var mode = from Pinjam in context.Pinjams
                       join Biodata in context.Biodatas
                       on Pinjam.Id equals id
                       select new PinjamModel()
                       {
                           IdUser = Pinjam.IdUser,
                           Id = Pinjam.Id,
                           User = Biodata.Username,
                           PBuku = Pinjam.PBuku,
                           WPinjam = DateTime.Now,
                           IdBuku = (int)Pinjam.IdBuku
                       };

            SP = mode.Distinct().ToList();

            int Id = 0, IdUser = 0, IdBuku = 0;
            string PBuku = "";

            foreach(var s in SP)
            {
                Id = s.Id;
                IdUser = s.IdUser;
                PBuku = s.PBuku;
                IdBuku = s.IdBuku;
            }


            SPinjam spinjam = new SPinjam()
            {
                IdUser = IdUser,
                PBuku = PBuku,
                IdBuku = IdBuku,
                Status = "Dipinjam",
                WKembali = DateTime.Now.AddDays(7)
            };

            DBuku book = context.DBukus.Where(some => some.Id == IdBuku).Single<DBuku>();
            book.Jumlah = book.Jumlah - 1;

            context.SPinjams.InsertOnSubmit(spinjam);
            context.SubmitChanges();


            Pinjam buku = context.Pinjams.Where(some => some.Id == id).Single<Pinjam>();

            context.Pinjams.DeleteOnSubmit(buku);
            context.SubmitChanges();

            return RedirectToAction("Confirm");
        }

        public ActionResult Client(int id)
        {
            List<SPinjamModel> SP = new List<SPinjamModel>();
            var mode = from SPinjam in context.SPinjams
                       where SPinjam.Id == id
                       select new SPinjamModel()
                       {
                           IdUser = SPinjam.IdUser,
                           Id = SPinjam.Id,
                           PBuku = SPinjam.PBuku,
                           WKembali = DateTime.Now,
                           IdBuku = (int)SPinjam.IdBuku,
                           Status = SPinjam.Status
                       };

            SP = mode.ToList();

            int Id = 0, IdUser = 0, IdBuku = 0;
            string PBuku = "", Stat = "";

            foreach (var s in SP)
            {
                Id = s.Id;
                IdUser = s.IdUser;
                PBuku = s.PBuku;
                IdBuku = s.IdBuku;
                Stat = s.Status;
            }

            if(Stat == "Dibalikan")
            {
                return RedirectToAction("Confirm");
            }

            TBaca baca = new TBaca()
            {
                Buku = PBuku,
                ReWaktu = DateTime.Now,
                IdUser = IdUser
            };

            context.TBacas.InsertOnSubmit(baca);

            SPinjam buku = context.SPinjams.Where(some => some.Id == Id).Single<SPinjam>();
            buku.Status = "Dibalikan";

            /*int tmbh = 0;
            var Add = from tambah in context.DBukus where tambah.Id == Id select tambah;
            foreach (var tmb in Add)
                tmbh = tmb.Jumlah;*/

            DBuku book = context.DBukus.Where(some => some.Id == IdBuku).Single<DBuku>();
            book.Jumlah = book.Jumlah + 1;
            context.SubmitChanges();

            return RedirectToAction("Confirm");
        }


        public ActionResult Remove(int Id)
        {
            var saring = from SPinjam in context.SPinjams where SPinjam.Id == Id select new { SPinjam.Status };
            string hasil = "";
            foreach (var sa in saring)
                hasil = sa.Status;

            if(hasil == "Dipinjam")
            {
                return RedirectToAction("Confirm");
            }

            SPinjam Del = context.SPinjams.Where(some => some.Id == Id).Single<SPinjam>();
            context.SPinjams.DeleteOnSubmit(Del);
            context.SubmitChanges();
            return RedirectToAction("Confirm");
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
        
    }
}