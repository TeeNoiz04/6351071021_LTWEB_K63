using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using MvcBookStore.Models;
using System.Net;
namespace MvcBookStore.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
        QLBANSACHEntities data = new QLBANSACHEntities();

        // Lấy giỏ hàng
        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang == null)
            {
                // Nếu giỏ hàng chưa tồn tại thì khởi tạo listGiohang
                lstGiohang = new List<GioHang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        // Thêm hàng vào giỏ
        public ActionResult ThemGiohang(int iMasach, string strURL)
        {
            Console.WriteLine(iMasach);
            // Lấy ra Session giỏ hàng
            List<GioHang> lstGiohang = Laygiohang();

            // Kiểm tra sách này đã tồn tại trong Session["Giohang"] chưa?
            GioHang sanpham = lstGiohang.Find(n => n.iMasach == iMasach);
            if (sanpham == null)
            {
                // Nếu chưa tồn tại, tạo mới sản phẩm
                sanpham = new GioHang(iMasach);
                lstGiohang.Add(sanpham); // Thêm sản phẩm mới vào giỏ hàng
                return Redirect(strURL); // Chuyển hướng về trang hiện tại
            }
            else
            {
                // Nếu đã tồn tại, tăng số lượng sản phẩm
                sanpham.iSoluong++;
                return Redirect(strURL); // Chuyển hướng về trang hiện tại
            }

        }
        // Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                // Tổng số lượng bằng tổng số lượng của từng sản phẩm
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        // Tính tổng tiền
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                // Tổng tiền bằng tổng thành tiền của từng sản phẩm
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        // Xây dựng trang Giỏ hàng
        public ActionResult GioHang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore"); // Chuyển hướng về trang Index nếu giỏ hàng rỗng
            }
            ViewBag.Tongsoluong = TongSoLuong(); // Hiển thị tổng số lượng sản phẩm
            ViewBag.TongTien = TongTien(); // Hiển thị tổng tiền
            return View(lstGiohang); // Trả về danh sách giỏ hàng
        }

        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        // Xóa Giỏ hàng
        public ActionResult XoaGiohang(int iMaSP)
        {
            // Lấy giỏ hàng từ Session
            List<GioHang> lstGiohang = Laygiohang();

            // Kiểm tra sách đã có trong Session["Giohang"]
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);

            // Nếu tồn tại thì cho sửa số lượng
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasach == iMaSP);
                return RedirectToAction("GioHang");
            }

            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }

            return RedirectToAction("GioHang");
        }
        // Cập nhật Giỏ hàng
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            // Lấy giỏ hàng từ Session
            List<GioHang> lstGiohang = Laygiohang();

            // Kiểm tra sách đã có trong Session["Giohang"]
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);

            // Nếu tồn tại thì cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }

            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            // Lấy giỏ hàng từ Session
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "BookStore");
        }
        // Hiển thị View DatHang để cập nhật các thông tin cho Đơn hàng
        [HttpGet]
        public ActionResult DatHang()
        {
            // Kiểm tra đăng nhập
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "BookStore");
            }

            // Lấy giỏ hàng từ Session
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            // Thêm Đơn hàng
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<GioHang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDH = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.Add(ddh);
            data.SaveChanges();

            // Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.SoDH = ddh.SoDH;
                ctdh.Masach = item.iMasach;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;
                data.CHITIETDONTHANGs.Add(ctdh);
            }

            data.SaveChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "GioHang");
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }

    }
}