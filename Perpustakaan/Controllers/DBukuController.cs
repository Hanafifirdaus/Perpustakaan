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

        public ActionResult Create()
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

            if (Session[a] == null)
            {
                return RedirectToAction("Admin");
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

            if (Session[a] == null)
            {
                return RedirectToAction("Admin");
            }

            if(id == null)
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

        public ActionResult Admin()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Admin(LoginModel login)
        {
            var q = from p in context.Logins
                    where p.Username == login.Username
                    && p.Password == login.Password
                    select p;

            if (q.Any())
            {
                DateTime now = DateTime.Now;
                Auten auten = new Auten();
                {
                    auten.Username = login.Username;
                    auten.AutSes = MD5Hash(login.Username + login.Password + now);
                };

                context.Autens.InsertOnSubmit(auten);
                context.SubmitChanges();
                
                Session[login.Username] = MD5Hash(login.Username + login.Password + now);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Admin");
            }
        }

        public ActionResult Delete(int id)
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

            if (Session[a] == null)
            {
                return RedirectToAction("Admin");
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

            List<SPinjamModel> spinjams = new List<SPinjamModel>();
            var querys = from Biodata in context.Biodatas
                        join SPinjam in context.SPinjams on Biodata.Id equals SPinjam.IdUser
                        select new SPinjamModel
                        {
                            Id = SPinjam.Id,
                            User = Biodata.Username,
                            IdUser = SPinjam.IdUser,
                            PBuku = SPinjam.PBuku,
                            WKembali = SPinjam.WKembali ?? DateTime.Now.AddDays(7),
                            IdBuku = (int)SPinjam.IdBuku
                        };
            spinjams = querys.ToList();


            List<PinjamModel> pinjams = new List<PinjamModel>();
            var query = from Biodata in context.Biodatas
                        join Pinjam in context.Pinjams on Biodata.Id equals Pinjam.IdUser
                        select new PinjamModel
                        {
                            User = Biodata.Username,
                            PBuku = Pinjam.PBuku,
                            WPinjam = DateTime.Today,
                            IdUser = Pinjam.IdUser,
                            IdBuku = (int)Pinjam.IdBuku
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
                       on Pinjam.IdBuku equals id
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
                Id = Id,
                IdUser = IdUser,
                PBuku = PBuku,
                IdBuku = IdBuku,
                WKembali = DateTime.Today.AddDays(7)
            };
            context.SPinjams.InsertOnSubmit(spinjam);
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