using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.Security;
using WebAppDP.Models;
using System;
using System.IO;
//using MailKit.Net.Smtp;
//using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using BCrypt;
using System.Web.Helpers;
using System.Net;
using System.Net.Mail;
using System.Threading;



namespace WebAppDP.Controllers
{

    public class AccountController : Controller
    {
        private readonly db_digitalprintContext _context;

        //private readonly EmailService _emailService;
        public AccountController()
        {
            _context = new db_digitalprintContext();
            //_emailService = new EmailService("smtp.gmail.com", 587, "digiprint94@gmail.com", "DhinkanHollow69@#");

        }

        //View untuk Login Page, dengan Route url Account/Login
        [Route("Account/Login")]
        public ActionResult LoginView()
        {
            return View();
        }

        //Action dari proses Login
        [HttpPost]
        [Route("Account")]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            //Cek apakah username yang diinputkan user sama seperti username di database
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                //jika username tidak ditemukan maka page Login akan di reload yang menandakan login tidak berhasil
                ModelState.AddModelError("", "Invalid login attempt.");
                string script = @"<script>
                            Swal.fire({
                                icon: 'error',
                                title: 'Oops...',
                                html: 'Username atau password salah'
                            });
                        </script>";
                TempData["swalScript"] = script;
                return View("LoginView");
            }

            //Password di enkripsi menggunakan algoritma BCrypt sehingga perlu dilakukan pengecekan password terlebih dahulu
            bool passwordIsValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);

            if (!passwordIsValid)
            {
                //jika password yang dimasukkan tidak valid, maka login page akan di reload yang menandakan proses login tidak berhasil
                ModelState.AddModelError("", "Invalid login attempt.");
                string script = @"<script>
                            Swal.fire({
                                icon: 'error',
                                title: 'Oops...',
                                html: 'Username atau password salah'
                            });
                        </script>";
                    TempData["swalScript"] = script;
                return View("LoginView");

            }

            //Kondisi untuk mengecek akun users apakah sudah melakukan verifikasi
            if (!user.IsVerified) //Jika belum, maka ini akan di jalankan
            {

                string verificationScript = @"<script>
                    Swal.fire({
                        icon: 'warning',
                        title: 'Oops...',
                        html: 'Akun belum diverifikasi. Silakan cek email Anda untuk melakukan verifikasi akun.'
                    });
                </script>";
                TempData["swalScripts"] = verificationScript;


                TempData["UsernameLogin"] = model.Username;

                Thread.Sleep(2000);

                return RedirectToAction("Verification");
            }
            else //Jika users telah melakukan verifikasi maka bagian ini yang akan dijalankan
            {
                //Gnakan metode Cookie untuk login
                //Ambil Identitas user yang login dengan username dan simpan di cookie
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }, DefaultAuthenticationTypes.ApplicationCookie);
                var authManager = HttpContext.GetOwinContext().Authentication;

                try
                {
                    //jika singin berhasil maka akan di redirect ke Action LoginSuccess page /Account
                    authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                    string userId = User.Identity.GetUserId(); //baris ini untuk mengambil UserId 
                    return RedirectToAction("LoginSuccess"); //baris ini untuk mengarahkan halaman ke bagian LoginSuccess dengan Route /Account

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                    return View("LoginView");
                }
            }            
        }


        //Authorize digunakan agar siapa pun yang ingin mengakses halaman tersebut maka diperlukan untuk login terlebih dahulu
        [Authorize]
        [Route("Account")]
        public ActionResult LoginSuccess()
        {

            var hargaJenisProduct = _context.ProductHarga
            .Where(o => o.JenisProduct == "Banner 60cm x 160cm")
            .FirstOrDefault();

            var hargaFotoCopy = _context.ProductHarga
            .Where(o => o.JenisProduct == "FotoCopy/Print")
            .FirstOrDefault();

            var harga = hargaJenisProduct.HargaProduct;
            var hargaF = hargaFotoCopy.HargaProduct;

            ViewBag.hargaBanner = harga;
            ViewBag.hargaFotoCopy = hargaF;
            return View("LoginSuccess");
        }

        //memanggil halaman Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Verification(string userNames)
        {
            string username = TempData["UserName"] as string;
            

            string usernamelogin = TempData["UsernameLogin"] as string;
            //ViewBag.UserNameLogin = usernamelogin;

            ViewBag.Usernames = username ?? usernamelogin;
            return View();
        }

        public ActionResult VerificationAct(LoginViewModel model, string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if(user == null)
            {
                return RedirectToAction("Verification");
            }
            if (model.VerificationCode != user.VerificationCode)
            {
                return RedirectToAction("Verification");
            }
            user.IsVerified = true;
            _context.SaveChanges();


            return RedirectToAction("LoginView");
        }

        public async Task<JsonResult> ResendCode(string userName)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

                string newVerificationCode = new Random().Next(100000, 999999).ToString();
                user.VerificationCode = newVerificationCode;

                await _context.SaveChangesAsync();

                
                string fromMail = ConfigurationManager.AppSettings["FromMail"];
                string fromPassword = ConfigurationManager.AppSettings["FromPassword"];




                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "Verification Code";
                message.To.Add(new MailAddress(user.Email));
                message.Body = $"<html><body>Kode Verifikasi Anda Adalah: {newVerificationCode}</body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };
                smtpClient.Send(message);

                return Json(new { success = true, message = "Verification code resent successfully." });
            }

            return Json(new { success = false, message = "Resending verification code failed." });
        }


        

        //Action Register
        //[HttpPost]
        public async Task<ActionResult> RegisterAct(LoginViewModel userv, string username, string usermail)
        {
            if (ModelState.IsValid)
            {
                string verificationCode = new Random().Next(100000, 999999).ToString(); //generete kodeverifikasi menggunakan Random nomor 

                userv.VerificationCode = verificationCode; //Masukkan code verifikasi tadi ke dalam datamodel dari registrasi

                userv.Password = BCrypt.Net.BCrypt.HashPassword(userv.Password); //gunakan BCrypt algoritma untuk mengengkripsi password
                _context.Users.Add(userv); //simpan informasi ke dalam database
                await _context.SaveChangesAsync();

                var eml = _context.Users
                    .Where(u => u.Username == username)
                    .FirstOrDefault();

                //Mulai dari bagian ini sampai selesai, adalah code untuk mengirim verifikasi kode ke email users
                //kedua variable string ini adalah untuk mengambil username dan password Email pada Configurationmanager
                string fromMail = ConfigurationManager.AppSettings["FromMail"];
                string fromPassword = ConfigurationManager.AppSettings["FromPassword"];

                //Konfigurasi untuk proses pengiriman email menggunakan kredensial dari kedua variable string sebelumnya
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "Verification Code";
                message.To.Add(new MailAddress(eml.Email));
                message.Body = $"<html><body>Kode Verifikasi Anda Adalah: {eml.VerificationCode}</body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };
                smtpClient.Send(message); //gunakan function send untuk mengirimkan code verifikasi ke email users

                TempData["UserName"] = username;

                return RedirectToAction("Verification");
            }

            ModelState.AddModelError("KonfirmasiPassword", "Password and confirmation password do not match.");
            return View("Register");
        }


        //Authorize digunakan sehingga user harus login terlebih dahulu untuk bisa mengaksesnya
        //Action Logout
        [Authorize]
        public ActionResult Logout()
        {
            //Pada Action ini, Sistem akan menghapus Cookie 
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View("LoginView");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }


        //Action untuk Menampilkan Users Orders
        [Authorize]
        public ActionResult Orders()
        {
            //ambil username di cookie berdasarkan user yang login
            var userID = User.Identity.GetUserName();
            var order = (from o in _context.Order
                         where o.Username == userID
                         join p in _context.Payment on o.Id_pesanan equals p.Id_pesanan into gj
                         from subp in gj.DefaultIfEmpty()
                         //lakukan fungsi select untuk menampilkan data berdasarkan Username yang login
                         select new OrderViewModel
                         {
                             Id_pesanan = o.Id_pesanan,
                             NamaPemesan = o.NamaPemesan,
                             JenisProduct = o.JenisProduct,
                             UkuranDesain = o.UkuranDesain,
                             FileDesain = o.FileDesain,
                             MPembayaran = o.MPembayaran,
                             HasPayment = subp != null
                         }).ToList();
            return View(order);
        }


        //Action untuk hapus pesanan
        [Authorize]
        public ActionResult HapusPesanan(int id)
        {
            if (ModelState.IsValid)
            {
                //Hapus pesanan berdasarkan Id Pesanan
                var order = _context.Order
                .Where(o => o.Id_pesanan == id)
                .FirstOrDefault();

                if (order == null)
                {
                    return HttpNotFound();
                }
                //ambil namafile berdasarkan id Pesanan yang ingin dihapus
                var fileName = order.FileDesain;

                //lakukan konfirmasi
                if (order.JenisProduct == "Banner") {
                    
                    //jika jenis produknya banner maka akan menghapus file desainnya di server pada folder File
                    var filePath = Path.Combine(Server.MapPath("~/FileDesign/File/"), fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }else if(order.JenisProduct == "FotoCopy/Print")
                {

                    //jika jenis produknya Fotocopy maka akan menghapus file pdf di server pada folder FCPdf
                    var filePath = Path.Combine(Server.MapPath("~/FileDesign/FCPdf/"), fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.Order.Remove(order);
                _context.SaveChanges();

                return RedirectToAction("Orders");
            }
            return View();
        }
    }
}