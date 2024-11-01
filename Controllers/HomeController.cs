using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBookStore.Controllers
{
    public class HomeController : Controller
    {
        // GET: BookStore
        QLBANSACHEntities data = new QLBANSACHEntities();
        public ActionResult Index()
        {
            return View();
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
       
        public ActionResult Take()
        {
            // Lấy 5 quyển sách mới nhất
            var latestBooks = data.SACHes
                                  .OrderByDescending(s => s.Ngaycapnhat)
                                  .Take(5)
                                  .ToList();

            return View(latestBooks);
        }
        public ActionResult Topic()
        {
            var topic = from cd in data.CHUDEs select cd;
            return PartialView(topic);
        }
        public ActionResult NhaXuatBan()
        {
            var chude = from cd in data.NHAXUATBANs select cd;
            return PartialView(chude);
        }
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes
                       where s.Masach == id
                       select s;
            return View(sach.Single());
        }
    }
}