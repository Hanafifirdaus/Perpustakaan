using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Perpustakaan.Models
{
    public class LoginController : Controller
    {
        private OperationDataContext context = null;
        public LoginController()
        {
            context = new OperationDataContext();
        }

        public ActionResult Admin()
        {
            DBukuModel model = new DBukuModel();
            return View(model);
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}