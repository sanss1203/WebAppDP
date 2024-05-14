using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.Security;
using WebAppDP.Models;
using System;
using Newtonsoft.Json.Linq;
//using Microsoft.AspNetCore.Mvc;
using System.Web.UI.WebControls;

namespace WebAppDP.Controllers
{
    public class AdminController : Controller
    {
        //Code ini berfungsi untuk memanggil db_context 
        private readonly db_digitalprintContext _context;

        public AdminController()
        {
            _context = new db_digitalprintContext();
        }


        // GET: Admin
        //Gunakan url "Admin/Login" untuk mengakses halaman login page Admin
        [Route("Admin/Login")]
        public ActionResult Index()
        {
            return View("Admin");
        }

        //Action dari Login
        public async Task<ActionResult> AdminLoginAct(AdminLoginModel model)
        {
            // Melakukan pengecekan Username dan password apakah sesuai di dalam database melalui db _context
            var user = await _context.Admin.FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

            // Jika username dan password tidak sesuai maka kondisi ini akan diekseskusi
            if (user == null)
            {
                // Sistem akan menampilkan error Username atau password salah
                ModelState.AddModelError("", "Invalid login attempt.");
                string script = @"<script>
                     Swal.fire({
                         icon: 'error',
                         title: 'Oops...',
                         html: 'Username atau password salah'
                     });
                 </script>";
                TempData["swalScript"] = script;
                return RedirectToAction("Index");
            }

            // Ambil identitas User admin yang Login berdasarkan username dan simpan pada cookie
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, "Admin") }, DefaultAuthenticationTypes.ApplicationCookie);
            var authManager = HttpContext.GetOwinContext().Authentication;

            try
            {
                // Jika berhasil login maka User admin akan di redirect pada halaman Administration
                authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                string userId = User.Identity.GetUserName();
                return RedirectToAction("Administration");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "Admin")]
        [Route("Administration")]
        public ActionResult Administration()
        {
            //Pada halaman Administration Menampilkan semua pesanan pada tabel orders
            var userID = User.Identity.GetUserName();
            ViewBag.userID = userID;

            var order = (from o in _context.Order
                         join p in _context.Payment on o.Id_pesanan equals p.Id_pesanan into gj
                         from subp in gj.DefaultIfEmpty()
                         select new OrderPaymentViewModel
                         {
                             Id_pesanan = o.Id_pesanan,
                             NamaPemesan = o.NamaPemesan,
                             NoHP = o.NoHP,
                             TanggalPesanan = o.TanggalPesanan,
                             JenisProduct = o.JenisProduct,
                             UkuranDesain = o.UkuranDesain,
                             FileDesain = o.FileDesain,
                             MPembayaran = o.MPembayaran,
                             HasPayment = subp != null, //melakukan apakah pembayaran sudah di lakukan atau belum
                             Id_pembayaran = subp.Id_pembayaran,
                             HargaPembayaran = subp.HargaPembayaran,
                             BuktiTF = subp.BuktiTF
                         }).ToList();

            
            return View(order);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Rekening()
        {
            var rekeningView = _context.TransferBank
                .Select(x => new TransferBankModel
                {
                    Id_transfer = x.Id_transfer,
                    Nama_bank = x.Nama_bank,
                    No_rekening = x.No_rekening,
                    Nama_penerima = x.Nama_penerima
                }).ToList();

            return View(rekeningView);  
            
        }

 
        public ActionResult simpanRek(string Nama_penerima, string Nama_bank, string No_rekening)
        {
            var userID = User.Identity.GetUserName();

            var rekEntry = new TransferBankModel
            {
                Nama_bank = Nama_bank,
                No_rekening = No_rekening,
                Nama_penerima = Nama_penerima
            };

            _context.TransferBank.Add(rekEntry);
            _context.SaveChanges();

            return RedirectToAction("Rekening");
        }

        public ActionResult HapusNoRek(int id)
        {
            if (ModelState.IsValid)
            {
                var rekEntry = _context.TransferBank
                    .Where(e => e.Id_transfer == id)
                    .FirstOrDefault();

                if(rekEntry == null)
                {
                    return HttpNotFound();
                }

                _context.TransferBank.Remove(rekEntry);
                _context.SaveChanges();

            }

            return RedirectToAction("Rekening");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Gantihargaproduct()
        {

            var listHarga = _context.ProductHarga
                .Select(x => new ProductHargaModel
                {
                    JenisProduct = x.JenisProduct,
                    HargaProduct = x.HargaProduct,
                }).ToList();

            return View(listHarga);

        }
        [Authorize(Roles = "Admin")]
        public ActionResult simpanNewHarga(string JenisProduct, string HargaProduct)
        {
            var userID = User.Identity.GetUserName();

            // Cek apakah JenisProduct sudah ada di database
            var existingProduct = _context.ProductHarga.FirstOrDefault(p => p.JenisProduct == JenisProduct);

            // Jika JenisProduct sudah ada, kembalikan pesan kesalahan
            if (existingProduct != null)
            {
                var error = new { status = "error", message = $"Data dengan Jenis Product {JenisProduct} Sudah Ada" };
                // Atau Anda bisa merespons dengan pesan JSON atau metode lainnya yang sesuai
                return Json(error);
            }

            // JenisProduct belum ada di database, tambahkan data baru
            var hargaProductModel = new ProductHargaModel
            {
                JenisProduct = JenisProduct,
                HargaProduct = HargaProduct,
            };

            _context.ProductHarga.Add(hargaProductModel);
            _context.SaveChanges();

            var res = new { status = "success", message = "Data berhasil disimpan" };

            return Json(res);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult HapusDataHarga(string jenisProduct)
        {
            if (ModelState.IsValid)
            {
                var dataHarga = _context.ProductHarga
                    .Where(e => e.JenisProduct == jenisProduct)
                    .FirstOrDefault();

                if (dataHarga == null)
                {
                    var error = new { status = "error", message = $"Data dengan Jenis Product {jenisProduct} Tidak ditemukan" };
                    return Json(error);
                }

                _context.ProductHarga.Remove(dataHarga);
                _context.SaveChanges();

            }
            var res = new { status = "success", message = "Data berhasil dihapus" };
            return Json(res);
        }


        //Banner pada Admin bisa diakses sesuai dengan Route dibawah ini  "Admin/Banner"
        [Authorize(Roles = "Admin")]
        [Route("Admin/Banner")]
        public ActionResult TransferBank()
        {
            //Menampilkan semua pesanan berdasarkan Jenis Product == Banner
            var order = (from o in _context.Order
                        join p in _context.Payment on o.Id_pesanan equals p.Id_pesanan into gj
                        from subp in gj.DefaultIfEmpty()
                        where o.JenisProduct == "Banner"
                        select new OrderPaymentViewModel
                        {
                            Id_pesanan = o.Id_pesanan,
                            NamaPemesan = o.NamaPemesan,
                            NoHP = o.NoHP,
                            TanggalPesanan = o.TanggalPesanan,
                            JenisProduct = o.JenisProduct,                        
                            UkuranDesain = o.UkuranDesain,
                            FileDesain = o.FileDesain,
                            MPembayaran = o.MPembayaran,
                            HasPayment = subp != null,
                            Id_pembayaran = subp.Id_pembayaran,
                            HargaPembayaran = subp.HargaPembayaran,
                            BuktiTF = subp.BuktiTF
                        }).ToList();
            return View(order);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult FotoCopy()
        {
            //Menampilkan semua pesanan berdasarkan Jenis Produc == Fotocopy
            var order = (from o in _context.Order
                         join p in _context.Payment on o.Id_pesanan equals p.Id_pesanan into gj
                         from subp in gj.DefaultIfEmpty()
                         where o.JenisProduct == "FotoCopy/Print"
                         select new OrderPaymentViewModel
                         {
                             Id_pesanan = o.Id_pesanan,
                             NamaPemesan = o.NamaPemesan,
                             NoHP = o.NoHP,
                             TanggalPesanan = o.TanggalPesanan,
                             JenisProduct = o.JenisProduct,
                             UkuranDesain = o.UkuranDesain,
                             FileDesain = o.FileDesain,
                             MPembayaran = o.MPembayaran,
                             HasPayment = subp != null,
                             Id_pembayaran = subp.Id_pembayaran,
                             HargaPembayaran = subp.HargaPembayaran,
                             BuktiTF = subp.BuktiTF
                         }).ToList();
            return View(order);
        }



        [Authorize(Roles = "Admin")]
        public ActionResult JadwalService()
        {
            var servicesList = _context.Services
                .OrderByDescending(s => s.Tgl_masuk)
                .Select(s => new ServicesViewModel
                {
                    Id_services = s.Id_services,
                    Nama_Lengkap = s.Nama_Lengkap,
                    No_hp = s.No_hp,
                    Jnis_service = s.Jnis_service,
                    Jnis_Os = s.Jnis_Os,
                    Tgl_masuk = s.Tgl_masuk,
                    Estimasi_selesai = s.Estimasi_selesai,
                    Keterangan = s.Keterangan,
                    Status = s.Status
                    //Tgl_masuk = DateTime.ParseExact(s.Tgl_masuk, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                    //Estimasi_selesai = DateTime.ParseExact(s.Estimasi_selesai, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")

                })
                .ToList();
            //var servicesList = _context.Services.ToList();
            return View(servicesList);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult statusUpdate(int idz, string status)
        {
            var updateStatus = _context.Services
                .FirstOrDefault(s => s.Id_services == idz);

            updateStatus.Status = status;
            _context.SaveChanges();

            return RedirectToAction("JadwalService");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult hapusRequest(int idS)
        {

            var hapusStatus = _context.Services
              .FirstOrDefault(s => s.Id_services == idS);

            _context.Services.Remove(hapusStatus);
            _context.SaveChanges();
            return RedirectToAction("JadwalService");
           // return View();            
        }

        //Code ini merupakan PartialViews yang akan muncul pop up ketika user mengklik link Details Pada tabel 
        [HttpGet]
        public ActionResult GetPaymentDetails(int id_pesanan)
        {
            //Menampilkan pop up Details berdasarkan parameter Id_pesanan yang di klik oleh admin di dalam tabel 
            var order = (from o in _context.Order
                        join p in _context.Payment on o.Id_pesanan equals p.Id_pesanan into gj
                        from subp in gj.DefaultIfEmpty()
                        where o.Id_pesanan == id_pesanan
                        select new OrderPaymentViewModel
                        {
                            Id_pesanan = o.Id_pesanan,
                            NamaPemesan = o.NamaPemesan,
                            JenisProduct = o.JenisProduct,
                            UkuranDesain = o.UkuranDesain,
                            FileDesain = o.FileDesain,
                            MPembayaran = o.MPembayaran,
                            HasPayment = subp != null,
                            Id_pembayaran = subp.Id_pembayaran,
                            HargaPembayaran = subp.HargaPembayaran,
                            BuktiTF = subp.BuktiTF
                        }).FirstOrDefault();

            return PartialView("_GetPaymentDetails", order);


            //OrderPaymentViewModel payment = _context.Payment.Where(p => p.Id_pesanan == id_pesanan).FirstOrDefault();


            //return Json(order);


        }

        [Authorize(Roles = "Admin")]
        public ActionResult Logout()
        {
            //Fungsi Logout, sistem akan menghapus Cookie
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View("Admin");
        }

    }
}
