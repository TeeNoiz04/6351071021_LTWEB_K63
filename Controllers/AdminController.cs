using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using System.IO;
using System.Web.UI.WebControls;
using System.Data.Entity.Migrations;
using System.Data.Entity;

namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        QLBANSACHEntities data = new QLBANSACHEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sach(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(db.SACHes.ToList());


            return View(data.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Themmoisach()
        {
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        public ActionResult Themmoisach(SACH sach, HttpPostedFileBase fileUpload)
        {
            // Đưa dữ liệu vào dropdown
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }

            if (ModelState.IsValid)
            {
                // Tạo tên file duy nhất với timestamp
                var fileName = Path.GetFileNameWithoutExtension(fileUpload.FileName);
                var extension = Path.GetExtension(fileUpload.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                var path = Path.Combine(Server.MapPath("~/images"), uniqueFileName);

                // Lưu hình ảnh vào đường dẫn
                fileUpload.SaveAs(path);

                // Gán tên file vào thuộc tính Anhbia
                sach.Anhbia = uniqueFileName;

                // Lưu vào CSDL
                data.SACHes.Add(sach);
                data.SaveChanges();

                return RedirectToAction("Sach");
            }
            else
            {
                ViewBag.Thongbao = "Vui lòng điền đầy đủ thông tin và chọn ảnh bìa hợp lệ.";
            }


            return RedirectToAction("Sach");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["username"];
            var matkhau = collection["password"];

            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                // Gán giá trị cho đối tượng được tạo mới (ad)
                Admin ad = data.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);

                if (ad != null)
                {
                    // ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }

            return View();

        }
        // Hiển thị sản phẩm
        public ActionResult Chitietsach(int id)
        {
            // Lấy ra đối tượng sách theo mã
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(sach);
        }
        // Xóa sản phẩm
        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Truyền thông tin sách vào View để hiển thị xác nhận xóa
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult XacNhanXoa(int id)
        {
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            data.SACHes.Remove(sach);
            data.SaveChanges();

            return RedirectToAction("Sach");
        }
        //// Chỉnh sửa sản phẩm (GET)
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            // Lấy ra đối tượng sách theo mã
            SACH sach = data.SACHes.SingleOrDefault(n => n.Masach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Đưa dữ liệu vào dropdown
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Log hoặc hiển thị lỗi
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            // Re-populate dropdown lists
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(data.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(sach);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        return View(sach);
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    data.Entry(sach).State = (EntityState)System.Data.EntityState.Modified;
                    data.SaveChanges();
                }
                return RedirectToAction("Sach");
            }





        }
        public ActionResult ThongKe()
        {
            var booksByCategory = data.SACHes
            .GroupBy(s => s.CHUDE.TenChuDe)  // Assuming `Tenchude` is the category name in CHUDE
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToList();

            ViewBag.ChartLabels = booksByCategory.Select(b => b.Category).ToArray();
            ViewBag.ChartData = booksByCategory.Select(b => b.Count).ToArray();

            return View();
        }
       
    }
}