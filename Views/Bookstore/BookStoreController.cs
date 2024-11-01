﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBookStore.Models;
namespace MvcBookStore.Views.Bookstore
{
    public class BookStoreController : Controller
    {
        // GET: BookStore
        QLBANSACHEntities data = new QLBANSACHEntities();
        public ActionResult Index()
        {
            // Lấy 5 quyển sách mới nhất
            var latestBooks = data.SACHes
                                  .OrderByDescending(s => s.Ngaycapnhat)
                                  .Take(5)
                                  .ToList();

            return View(latestBooks);
     
        }
        public ActionResult Take() {
            // Lấy 5 quyển sách mới nhất
            var latestBooks = data.SACHes
                                  .OrderByDescending(s => s.Ngaycapnhat)
                                  .Take(5)
                                  .ToList();

            return View(latestBooks);
        }
        public ActionResult Topic() {
            var topic = from cd in data.CHUDEs select cd;
            return PartialView(topic);
        }
        public ActionResult NhaXuatBan()
        {
            var chude = from cd in data.NHAXUATBANs select cd;
            return PartialView(chude);
        }
        public ActionResult Details(int id) {
            var sach = from s in data.SACHes
                       where s.Masach == id
                       select s;
            return View(sach.Single());
        }

        public ActionResult SPTheoChude(int id)
        {
            var sach = from s in data.SACHes where s.MaCD == id select s;
            return View(sach);
        }
        public ActionResult SPTheoNXB(int id)
        {
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return View(sach);
        }

    }
}