using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppDP.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Globalization;
using PdfSharp.Pdf;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Web.Helpers;
using MailKit.Search;

namespace WebAppDP.Controllers
{
    public class ProductController : Controller
    {

        private readonly db_digitalprintContext _context;
       

        public ProductController()
        {
            _context = new db_digitalprintContext();
        }
        // GET: Product/Details/{id}
        [HttpGet]
        [Authorize]
        public ActionResult Details(int id = 1)
        {
            var product = new ProductModel { Id = id, Name = "Product " + id };
            var model = new OrderViewModel();
           
            var userID = User.Identity.GetUserName();
            ViewBag.UserID = userID;
            var daftarAlamat = _context.Alamat
                .Where(a => a.Username == userID)
                .Select(o => o.AlamatDetail)
                .ToList();

            var alamatList = new SelectList(daftarAlamat);

            var bannerHargaList = _context.ProductHarga
                .Where(p => p.JenisProduct != "FotoCopy/Print")
                .Select(p => new SelectListItem { Text = $"{p.JenisProduct}, {p.HargaProduct}", Value = p.JenisProduct })
                .ToList();

            var bannerJenisHargaList = new SelectList(bannerHargaList, "Value", "Text");

            var namaLengkap = _context.Users
                .Where(u => u.Username == userID)
                .Select(a => a.NamaLengkap)
                .FirstOrDefault();

            string jenisProduct = "";
            string viewName = "";
            switch (id)
            {
                case 1:
                    
                    viewName = "Banner";
                    model.AlamatList = alamatList;
                    model.NamaPemesan = namaLengkap;
                    model.JenisProductHargaList = bannerJenisHargaList;
                    jenisProduct = "Banner";

                    break;
                case 2:

                    viewName = "FotoCopy";
                    model.AlamatList = alamatList;
                    model.NamaPemesan = namaLengkap;
                    jenisProduct = "FotoCopy/Print";
                    break;

                case 3:
                    //viewName = "Service";
               
                    var servStatus = _context.Services
                        .Where(s => s.Username == userID)
                        .Select(s => new ServicesViewModel
                        {
                            Nama_Lengkap = s.Nama_Lengkap,
                            No_hp = s.No_hp,
                            Jnis_service = s.Jnis_service,
                            Jnis_Os = s.Jnis_Os,
                            Tgl_masuk = s.Tgl_masuk,
                            Estimasi_selesai = s.Estimasi_selesai,
                            Keterangan = s.Keterangan,
                            Status = s.Status
                        }).ToList();
                    return View("Service", servStatus);

                    break;
            }

          
            model.JenisProduct = jenisProduct;

            return View(viewName, model);
        }


        public JsonResult getDataAlamat()
        {
            var userID = User.Identity.GetUserName();
            var alamatLists = _context.Alamat
                    .Where(a => a.Username == userID)
                    .ToList();
            return Json(alamatLists, JsonRequestBehavior.AllowGet);
        }




        //Code ini untuk memelakukan pembayaran melalui halaman dashboard users
        [Authorize]
        public ActionResult PaymentFA(int id)
        {
            //userID mengambil username dari users yang sedang login
            var userID = User.Identity.GetUserName();
            var order = _context.Order
                .Where(o => o.Username == userID && o.Id_pesanan == id)
                .OrderByDescending(o => o.Id_pesanan == id)
                .FirstOrDefault();

            var hargaJenisProduct = _context.ProductHarga
                .FirstOrDefault(p => p.JenisProduct == order.UkuranDesain);

            var ongkir = _context.Alamat
               .Where(o => o.AlamatDetail == order.Alamat && o.Username == userID)
               .OrderByDescending(o => o.Id_ongkir)
               .FirstOrDefault();

            decimal jumlahOngkir = decimal.Parse(ongkir.HargaOngkir);

            if (order.JenisProduct == "Banner" && order.MPembayaran == "Transfer Bank")
            {
                decimal harga = 0;
                decimal hargaProduk = decimal.Parse(hargaJenisProduct.HargaProduct);
                if (order.UkuranDesain == hargaJenisProduct.JenisProduct)
                {
                    harga = hargaProduk + jumlahOngkir;
                }
                else if (order.UkuranDesain == hargaJenisProduct.JenisProduct)
                {
                    harga = hargaProduk + jumlahOngkir;
                }
                else if (order.UkuranDesain == hargaJenisProduct.JenisProduct)
                {
                    harga = hargaProduk + jumlahOngkir;
                }

                if (order == null)
                {
                    return RedirectToAction("Details", new { id = 1 });
                }
                var paymentViewModel = new PaymentViewModel
                {
                    Id_pesanan = order.Id_pesanan,
                    NamaPemesan = order.NamaPemesan,
                    UkuranDesain = order.UkuranDesain,
                    MPembayaran = order.MPembayaran,
                    JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID"))
                };

                paymentViewModel.HargaPembayaran = harga.ToString("C0", new CultureInfo("id-ID"));

                ViewBag.Harga = harga.ToString("C0", new CultureInfo("id-ID"));

                return View("Payment", paymentViewModel);
            } else if(order.JenisProduct == "Banner" && order.MPembayaran == "COD")
            {


                decimal harga = 0;
                if (order.UkuranDesain == "60cm x 160cm, Rp.120.000")
                {
                    harga = 120000 + jumlahOngkir;
                }
                else if (order.UkuranDesain == "80cm x 200cm, Rp.150.000")
                {
                    harga = 150000 + jumlahOngkir;
                }
                else if (order.UkuranDesain == "100cm x 250cm, Rp.200.000")
                {
                    harga = 200000 + jumlahOngkir; 
                }

                if (order == null)
                {
                    return RedirectToAction("Details", new { id = 1 });
                }
                var paycod = new PaymentViewModel
                {
                    Id_pesanan = order.Id_pesanan,
                    NamaPemesan = order.NamaPemesan,
                    UkuranDesain = order.UkuranDesain,
                    MPembayaran = order.MPembayaran,
                    JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID"))
                };

                paycod.HargaPembayaran = harga.ToString("C0", new CultureInfo("id-ID"));

                ViewBag.Harga = harga.ToString("C0", new CultureInfo("id-ID"));

                return View("CodView", paycod);
            }else if(order.JenisProduct == "FotoCopy/Print" && order.MPembayaran == "Transfer Bank")
            {
                var lastOrder = _context.Order
                    .Where(o => o.Username == userID)
                    .OrderByDescending(o => o.Id_pesanan)
                    .FirstOrDefault();

                var ongkirr = _context.Alamat
                    .Where(o => o.AlamatDetail == lastOrder.Alamat && o.Username == userID)
                    .OrderByDescending(o => o.Id_ongkir)
                    .FirstOrDefault();

                var fileNames = lastOrder.FileDesain.Split(';');

                int totalPageCount = 0;

                foreach (var fileName in fileNames)
                {
                    var pdfPath = Path.Combine(Server.MapPath("~/FileDesign/FCPdf"), fileName);
                    var document = PdfSharp.Pdf.IO.PdfReader.Open(pdfPath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                    int pageCount = document.PageCount; // Hitung semua halaman pada file pdf yang di upload
                    totalPageCount += pageCount;
                }

                decimal harga = totalPageCount * 500; // tetapkan harga foto copy per lembar 500 
                                                      // lalu kalikan dengan jumlah halaman
                var payment = new PaymentViewModel
                {
                    Id_pesanan = lastOrder.Id_pesanan,
                    NamaPemesan = lastOrder.NamaPemesan,
                    UkuranDesain = lastOrder.UkuranDesain,
                    JumlahCopy = lastOrder.JumlahCopy,
                    JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID")),
                    MPembayaran = lastOrder.MPembayaran
                };

                jumlahOngkir = decimal.Parse(ongkir.HargaOngkir); // ambil jumlah ongkir
                                                                          //kalikan terlebih dahulu harga dengan jumlah copy(untuk fotocopy) ditambah dengan jumlahongkir
                decimal finalHarga = harga * payment.JumlahCopy + jumlahOngkir;

                payment.JumlahHalaman = totalPageCount.ToString();
                payment.HargaPembayaran = finalHarga.ToString("C0", new CultureInfo("id-ID"));

                ViewBag.JumlahHal = totalPageCount.ToString();
                ViewBag.Harga = finalHarga.ToString("C0", new CultureInfo("id-ID"));

                return View("PaymentFC", payment);

               // return View("PaymentFC", payment);
            }else if(order.JenisProduct == "FotoCopy/Print" && order.MPembayaran == "COD")
            {
                var fileName = order.FileDesain;
                var pdfPath = Path.Combine(Server.MapPath("~/FileDesign/FCPdf"), fileName);
                var document = PdfSharp.Pdf.IO.PdfReader.Open(pdfPath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                int pageCount = document.PageCount;

                decimal harga = pageCount * 500;

                var paymentCod = new PaymentViewModel
                {
                    Id_pesanan = order.Id_pesanan,
                    NamaPemesan = order.NamaPemesan,
                    UkuranDesain = order.UkuranDesain,
                    JumlahCopy = order.JumlahCopy,
                    MPembayaran = order.MPembayaran,
                    JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID"))

                };

                decimal finalHarga = harga * order.JumlahCopy + jumlahOngkir;

                paymentCod.JumlahHalaman = pageCount.ToString();
                paymentCod.HargaPembayaran = finalHarga.ToString("C0", new CultureInfo("id-ID"));


                ViewBag.JumlahHal = pageCount.ToString();
                ViewBag.Harga = finalHarga.ToString("C0", new CultureInfo("id-ID"));

                return View("PaymentFCod", paymentCod);
            }

            return View();
        }

        [Authorize]
        public ActionResult Payment(int id)
        {

            var userID = User.Identity.GetUserName(); //ambil UserId dari pengguna yang login
            OrderViewModel lastoreder = _context.Order //Ambil pesannannya berdasarkan userId dan Id_Pesanan
                .Where(o => o.Username == userID && o.Id_pesanan == id)
                .FirstOrDefault();


            var hargaJenisProduct = _context.ProductHarga
                .FirstOrDefault(p => p.JenisProduct == lastoreder.UkuranDesain);

            //Tampilkan Bank tujuan
            var rek = _context.TransferBank 
                           .Select(x => new TransferBankModel
                           {
                               Nama_bank = x.Nama_bank,
                               No_rekening = x.No_rekening,
                               Nama_penerima = x.Nama_penerima
                           }).FirstOrDefault();

            ViewBag.NamaBank = rek.Nama_bank;
            ViewBag.NomorRekening = rek.No_rekening;
            ViewBag.NamaPnerima = rek.Nama_penerima;

            var ongkir = _context.Alamat
                .Where(o => o.Username == userID && o.AlamatDetail == lastoreder.Alamat)
                .OrderByDescending(o => o.Id_ongkir)
                .FirstOrDefault();

            decimal jumlahOngkir = decimal.Parse(ongkir.HargaOngkir);

            decimal harga = 0;
            decimal hargaProduk = decimal.Parse(hargaJenisProduct.HargaProduct);
            //tetapkan harga pada setiap ukuran desain banner
            if (lastoreder.UkuranDesain == hargaJenisProduct.JenisProduct)
            {
                harga = hargaProduk + jumlahOngkir;
            }
            else if (lastoreder.UkuranDesain == hargaJenisProduct.JenisProduct)
            {
                harga = hargaProduk + jumlahOngkir;
            }
            else if (lastoreder.UkuranDesain == hargaJenisProduct.JenisProduct)
            {
                harga = hargaProduk + jumlahOngkir;
            }

            if (lastoreder == null)
            {
                return RedirectToAction("Details", new { id = 1 });
            }
            var paymentViewModel = new PaymentViewModel
            {
                Id_pesanan = lastoreder.Id_pesanan,
                NamaPemesan = lastoreder.NamaPemesan,
                UkuranDesain = lastoreder.UkuranDesain,
                MPembayaran = lastoreder.MPembayaran,
                JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID"))
               
        };
            paymentViewModel.HargaPembayaran = harga.ToString("C0", new CultureInfo("id-ID"));
            ViewBag.Harga = paymentViewModel.HargaPembayaran; //Tampilkan di laayayr harga Pembayarannya
            return View("Payment", paymentViewModel);
        }

        [Authorize]
        public ActionResult PaymentCOD(int id)
        {

            var userID = User.Identity.GetUserName();
            OrderViewModel lastorder = _context.Order
                .Where(o => o.Username == userID && o.Id_pesanan == id)
                .FirstOrDefault();

            var hargaJenisProduct = _context.ProductHarga
                .FirstOrDefault(p => p.JenisProduct == lastorder.UkuranDesain);

            var ongkir = _context.Alamat
                .Where(o => o.AlamatDetail == lastorder.Alamat && o.Username == userID)
                .OrderByDescending(o => o.Id_ongkir)
                .FirstOrDefault();

            decimal jumlahOngkir = decimal.Parse(ongkir.HargaOngkir);
            decimal hargaProduk = decimal.Parse(hargaJenisProduct.HargaProduct);
            decimal harga = 0;
            if (lastorder.UkuranDesain == hargaJenisProduct.JenisProduct)
            {
                harga = hargaProduk + jumlahOngkir;
            }
            else if (lastorder.UkuranDesain == hargaJenisProduct.JenisProduct)
            {
                harga = hargaProduk + jumlahOngkir;
            }
            else if (lastorder.UkuranDesain == hargaJenisProduct.JenisProduct)
            {
                harga = hargaProduk + jumlahOngkir;
            }

            if (lastorder == null)
            {
                return RedirectToAction("Details", new { id = 1 });
            }
            var paycod = new PaymentViewModel
            {
                Id_pesanan = lastorder.Id_pesanan,
                NamaPemesan = lastorder.NamaPemesan,
                UkuranDesain = lastorder.UkuranDesain,
                MPembayaran = lastorder.MPembayaran,
                JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID")),
                HargaPembayaran = harga.ToString("C0", new CultureInfo("id-ID"))
            };

            //paycod.HargaPembayaran = harga.ToString("C0", new CultureInfo("id-ID"));

            //ViewBag.Harga = harga.ToString("C0", new CultureInfo("id-ID"));

            return View("CodView", paycod);
        }

        //Action dari pembayaran transfer bank dengan jenis product Banner

        [Authorize]
        public ActionResult PaymentFC(int id)
        {
            var userID = User.Identity.GetUserName(); //Ambil UserId dari users yang login


            OrderViewModel lastorder = _context.Order
                .Where(o => o.Username == userID && o.Id_pesanan == id) //Ambil pesanannya Berdasarkan Id Pesanan dan UserID
                .FirstOrDefault();

            var hargaJenisProduct = _context.ProductHarga
                .FirstOrDefault(p => p.JenisProduct == lastorder.JenisProduct);

            //Tampilkan Bank tujuan
            var getRek = _context.TransferBank
                .Select(x => new TransferBankModel
                {
                    Nama_bank = x.Nama_bank,
                    No_rekening = x.No_rekening,
                    Nama_penerima = x.Nama_penerima
                }).FirstOrDefault();
                ViewBag.NmaBank = getRek.No_rekening;
                ViewBag.noRek = getRek.Nama_bank;
                ViewBag.nmP = getRek.Nama_penerima;


            var ongkir = _context.Alamat
                .Where(o => o.AlamatDetail == lastorder.Alamat && o.Username == userID)
                .OrderByDescending(o => o.Id_ongkir)
                .FirstOrDefault();

            var fileNames = lastorder.FileDesain.Split(';');

            int totalPageCount = 0;

            //Ambil File pdf yang di upload
            foreach (var fileName in fileNames)
            {
                var pdfPath = Path.Combine(Server.MapPath("~/FileDesign/FCPdf"), fileName);
                var document = PdfSharp.Pdf.IO.PdfReader.Open(pdfPath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                int pageCount = document.PageCount; // Hitung semua halaman pada file pdf yang di upload
                totalPageCount += pageCount;
            }

            decimal hargaProduk = decimal.Parse(hargaJenisProduct.HargaProduct);

            decimal harga = totalPageCount * hargaProduk; // tetapkan harga foto copy per lembar 500 
                                                  // lalu kalikan dengan jumlah halaman
            var payment = new PaymentViewModel
            {
                Id_pesanan = lastorder.Id_pesanan,
                NamaPemesan = lastorder.NamaPemesan,
                UkuranDesain = lastorder.UkuranDesain,
                JumlahCopy = lastorder.JumlahCopy,
                JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID")),
                MPembayaran = lastorder.MPembayaran
            };

            decimal jumlahOngkir = decimal.Parse(ongkir.HargaOngkir); // ambil jumlah ongkir
            //kalikan terlebih dahulu harga dengan jumlah copy(untuk fotocopy) ditambah dengan jumlahongkir
            decimal finalHarga = harga * payment.JumlahCopy + jumlahOngkir;

            payment.JumlahHalaman = totalPageCount.ToString();
            payment.HargaPembayaran = finalHarga.ToString("C0", new CultureInfo("id-ID"));

            ViewBag.JumlahHal = totalPageCount.ToString();
            ViewBag.Harga = finalHarga.ToString("C0", new CultureInfo("id-ID"));

            return View("PaymentFC", payment);
        }


        //Action dari konfirmasi jenis pembayaran COD dengan jenis product Banner
        [Authorize]
        public ActionResult PaymentFCod(int id)
        {

            var userID = User.Identity.GetUserName();
            OrderViewModel lastorder = _context.Order
                .Where(o => o.Username == userID && o.Id_pesanan == id)
                .FirstOrDefault();

            var ongkir = _context.Alamat
               .Where(o => o.AlamatDetail == lastorder.Alamat && o.Username == userID)
               .OrderByDescending(o => o.Id_ongkir)
               .FirstOrDefault();

            var fileNames = lastorder.FileDesain.Split(';');

            int totalPageCount = 0;

            foreach (var fileName in fileNames)
            {
                var pdfPath = Path.Combine(Server.MapPath("~/FileDesign/FCPdf"), fileName);
                var document = PdfSharp.Pdf.IO.PdfReader.Open(pdfPath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                int pageCount = document.PageCount; // Hitung semua halaman pada file pdf yang di upload
                totalPageCount += pageCount;
            }

            decimal harga = totalPageCount * 500;
    

            var paymentCod = new PaymentViewModel
            {
                Id_pesanan = lastorder.Id_pesanan,
                NamaPemesan = lastorder.NamaPemesan,
                UkuranDesain = lastorder.UkuranDesain,
                JumlahCopy = lastorder.JumlahCopy,
                JumlahOngkir = decimal.Parse(ongkir.HargaOngkir).ToString("C0", new CultureInfo("id-ID")),
                MPembayaran = lastorder.MPembayaran
            };

            decimal jumlahOngkir = decimal.Parse(ongkir.HargaOngkir);
            decimal finalHarga = harga * paymentCod.JumlahCopy + jumlahOngkir;

            paymentCod.JumlahHalaman = totalPageCount.ToString();
            paymentCod.HargaPembayaran = finalHarga.ToString("C0", new CultureInfo("id-ID"));


            ViewBag.JumlahHal = totalPageCount.ToString();
            ViewBag.Harga = finalHarga.ToString("C0", new CultureInfo("id-ID"));

            return View("PaymentFCod",paymentCod);
        }

        //Action dari konfirmasi COD dengan jenis product FotoCopy
        [HttpPost]
        [Authorize]
        public ActionResult ActionCodFC(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userID = User.Identity.GetUserName();
                var orderGet = _context.Order
                    .FirstOrDefault(o => o.Username == userID && o.Id_pesanan == model.Id_pesanan);

                if (orderGet != null)
                {
                    PaymentViewModel paymentCod = new PaymentViewModel
                    {
                        Id_pesanan = model.Id_pesanan,
                        NamaPemesan = model.NamaPemesan,
                        UkuranDesain = model.UkuranDesain,
                        JumlahHalaman = model.JumlahHalaman,
                        JumlahCopy = model.JumlahCopy,
                        MPembayaran = orderGet.MPembayaran,
                        JumlahOngkir = model.JumlahOngkir,
                        HargaPembayaran = model.HargaPembayaran
                    };

                    _context.Payment.Add(paymentCod);
                    _context.SaveChanges();

                    var responses = new { message = "Pesanan berhasil di Konfirmasi", redirectUrlFC = Url.Action("Details", new { id = 2 }) };
                    return Json(responses);
                }
            }

            return View(model);
        }



        //Action dari Pembayaran FotoCopy dengan metode pembayaran Transfer Bank
        [HttpPost]
        [Authorize]
        public ActionResult ActionPaymentFC(PaymentViewModel model, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {

                var userID = User.Identity.GetUserName();
                var orderGet = _context.Order
                    .Where(o => o.Username == userID && o.Id_pesanan == model.Id_pesanan)
                    .FirstOrDefault();
                string path = Server.MapPath("~/FileDesign/BuktiTF");

                string fileName = Path.GetFileName(file.FileName);
                string fullPath = Path.Combine(path, fileName);

                file.SaveAs(fullPath); //simpan bukti transfer yang sudah diinputkan oleh user
                
                PaymentViewModel paymentfc = new PaymentViewModel
                {
                    Id_pesanan = model.Id_pesanan,
                    NamaPemesan = model.NamaPemesan,
                    UkuranDesain = model.UkuranDesain,
                    JumlahHalaman = model.JumlahHalaman,
                    JumlahCopy = model.JumlahCopy,
                    MPembayaran = orderGet.MPembayaran,
                    JumlahOngkir = model.JumlahOngkir,
                    HargaPembayaran = model.HargaPembayaran,
                    BuktiTF = fileName
                };

                _context.Payment.Add(paymentfc);
                _context.SaveChanges();

                var responses = new { message = "Pesanan berhasil di Konfirmasi", redirectUrlsF = Url.Action("Details", new { id = 2 }) };
                return Json(responses);
                //return RedirectToAction("Details", new { id = 2 });
            }
            return View();
        }
        //Action untuk pembayaran COD dengan jenis product Banner
        
        [Authorize]
        public ActionResult BannerPayCOD(PaymentViewModel model)
        {
            var userID = User.Identity.GetUserName(); //ambil username dari user yang login

            var order = _context.Order
                .Where(o => o.Username == userID && o.Id_pesanan == model.Id_pesanan)
                .FirstOrDefault();


            PaymentViewModel pay = new PaymentViewModel
            {

                Id_pesanan = model.Id_pesanan,
                NamaPemesan = model.NamaPemesan,
                UkuranDesain = model.UkuranDesain,
                MPembayaran = order.MPembayaran,
                JumlahOngkir = model.JumlahOngkir,
                HargaPembayaran = model.HargaPembayaran
            };

            _context.Payment.Add(pay);
            _context.SaveChanges();

            var responses = new { message = "Pesanan berhasil di Konfirmasi", resultUrlBC = Url.Action("Details", new { id = 1 }) };
            return Json(responses);
        }

        //Views dari metode pembayaran Transfer bank dengan jenis product Banner
        [Authorize]
        [HttpPost]
        public ActionResult PaymentAct(PaymentViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var userID = User.Identity.GetUserName();

                var orderGet = _context.Order
                    .Where(o => o.Username == userID && o.Id_pesanan == model.Id_pesanan)
                    .FirstOrDefault();

                string path = Server.MapPath("~/FileDesign/BuktiTF");
                string fileName = Path.GetFileName(file.FileName);
                string fullPath = Path.Combine(path, fileName);

                file.SaveAs(fullPath);

                PaymentViewModel payment = new PaymentViewModel
                {
                    Id_pesanan = model.Id_pesanan,
                    NamaPemesan = model.NamaPemesan,
                    UkuranDesain = model.UkuranDesain,
                    MPembayaran = orderGet.MPembayaran,
                    JumlahOngkir = model.JumlahOngkir,
                    HargaPembayaran = model.HargaPembayaran,
                    BuktiTF = fileName
                };

                _context.Payment.Add(payment);
                _context.SaveChanges();

                var responses = new { message = "Pesanan berhasil di Konfirmasi", redirectUrls = Url.Action("Details", new { id = 1 }) };
                return Json(responses);

            }
            return View();
        }
        //Action dari buat Pesanan 
 
        [HttpPost]
        public async Task<ActionResult> BuatPesanan(OrderViewModel model, HttpPostedFileBase file, IEnumerable<HttpPostedFileBase> multfile)
        {
            //ModelState, dilalkukan pengecekan apakah model yang dikirimkan dari view itu sama dengan yang digunakan di controller
            if (ModelState.IsValid)
            {
                //string path = ""; //deklarasi variabel path atau lokasi penyimpanan data
                string fileName = "";
                string path = "";
                if (model.JenisProduct == "Banner")
                {
                    int maxDataSize = 15728640;
                    if (file != null && file.ContentLength > 0 && file.ContentLength <= maxDataSize)
                    {
                        path = Server.MapPath("~/FileDesign/File"); //Jika JenisProductnya Banner maka lokasi untuk filenya ada di /FileDesign/File
                        fileName = Path.GetFileName(file.FileName);

                        file.SaveAs(Path.Combine(path, fileName));
                    }

                }
                else if (model.JenisProduct == "FotoCopy/Print")
                {
                    //Path.GetExtension(file.FileName).ToLower() == ".pdf"
                    int maxDataSize = 5242880;
                    path = Server.MapPath("~/FileDesign/FCPdf");
                    foreach (var fl in multfile)
                    {
                        if (fl != null && fl.ContentLength > 0 && fl.ContentLength <= maxDataSize)
                        {
                            string fileNames = Path.GetFileName(fl.FileName);
                            fl.SaveAs(Path.Combine(path, fileNames));

                            if (fileName != "")
                            {
                                fileName += ";";
                            }
                            fileName += fileNames;
                        }
                        else
                        {
                            ModelState.AddModelError("", "Please choose valid PDF files to upload");
                            return View(model);
                        }
                    }
                }
                
                var userId = User.Identity.GetUserName(); //Ambil username dari user yang sedang login

                    var user = _context.Users.FirstOrDefault(u => u.Username == userId);
                    if (user == null)
                    {
                        return View("Banner");
                    }
                //input data beradarkan model dan di form pada view
                OrderViewModel order = new OrderViewModel()
                    {
                        NamaPemesan = model.NamaPemesan,
                        Alamat = model.Alamat,
                        NoHP = model.NoHP,
                        JenisProduct = model.JenisProduct,
                        UkuranDesain = model.UkuranDesain,
                        TanggalPesanan = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FileDesain = fileName,
                        JumlahCopy = model.JumlahCopy,
                        Keterangan = model.Keterangan,
                        MPembayaran = model.MPembayaran,
                        Username = user.Username
                        
                    };
                // Add order to database
                    //TempData["OrderData"] = order;
                    _context.Order.Add(order); //simpan data di dalam database
                    _context.SaveChanges();
                //var redirectRouteValues = new RouteValueDictionary(new { Id_pesanan = orderId, Alamat = alamat });


                //Kondisi ini adalah untuk menentukan Jenis Pesanan dan Jenis Transaksi yang dipilih oleh users
                if (model.MPembayaran == "Transfer Bank")
                {
                    if (model.JenisProduct == "Banner")
                    {

                        //return RedirectToAction("Payment", order);
                        var successResponse = new { message = "Pesanan berhasil disimpan", redirectUrl = Url.Action("Payment", new { id = order.Id_pesanan }) };
                        return Json(successResponse);
                    }
                    else if (model.JenisProduct == "FotoCopy/Print")
                    {
                        var successResponse = new { message = "Pesanan berhasil disimpan", redirectUrl = Url.Action("PaymentFC", new { id = order.Id_pesanan }) };
                        return Json(successResponse);
                    }
                }
                else if (model.MPembayaran == "COD")
                {
                    if (model.JenisProduct == "Banner")
                    {
                        var successResponse = new { message = "Pesanan berhasil disimpan", redirectUrl = Url.Action("PaymentCOD", new { id = order.Id_pesanan }) };
                        return Json(successResponse);
                    }
                    else if (model.JenisProduct == "FotoCopy/Print")
                    {
                        var successResponse = new { message = "Pesanan berhasil disimpan", redirectUrl = Url.Action("PaymentFCod", new { id = order.Id_pesanan }) };
                        return Json(successResponse);
                    }
                }

            }
            return View();
        }

        public ActionResult simpanAlamat(string alamatDetail, double latitude, double longitude, string hasilOngkir)
        {
            var userID = User.Identity.GetUserName();

            var alamatEntity = new AlamatViewModel
            {
                AlamatDetail = alamatDetail,
                Username = userID,
                Latitude = latitude,
                Longitude = longitude,
                HargaOngkir = hasilOngkir
            };
            //Save semua data ke database
            _context.Alamat.Add(alamatEntity);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = 1 });
            //var response = new { message = 200 };
            //return Json(response);
        }

        public ActionResult simpanAlamat2(string alamatDetail, double latitude, double longitude, string hasilOngkir)
        {
            var userID = User.Identity.GetUserName();

            var alamatEntity = new AlamatViewModel
            {
                AlamatDetail = alamatDetail,
                Username = userID,
                Latitude = latitude,
                Longitude = longitude,
                HargaOngkir = hasilOngkir
            };
            //Save semua data ke database
            _context.Alamat.Add(alamatEntity);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = 2 });
            //var response = new { message = 200 };
            //return Json(response);
        }


        public JsonResult hapusAlamat(int id_ongkir)
        {
            var userID = User.Identity.GetUserName();

            var ongkir = _context.Alamat
                    .Where(a => a.Username == userID && a.Id_ongkir == id_ongkir)
                    .FirstOrDefault();

            _context.Remove(ongkir);
            _context.SaveChanges();

            var res = new { status = "success", message = "Data berhasil dihapus" };
            return Json(res);
        }
    }
}
