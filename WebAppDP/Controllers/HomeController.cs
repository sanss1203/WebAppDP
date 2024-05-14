using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppDP.Models;

namespace WebAppDP.Controllers
{
    public class HomeController : Controller
    {
        private readonly db_digitalprintContext _context;
        
        public HomeController()
        {
            _context = new db_digitalprintContext();
        }
        public ActionResult Index()
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
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Message(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                ContactViewModel message = new ContactViewModel()
                {
                    NamaPengirim = model.NamaPengirim,
                    EmailPengirim = model.EmailPengirim,
                    Subject = model.Subject,
                    Pesan = model.Pesan
                };

                _context.Messages.Add(message);
                _context.SaveChanges();

                return RedirectToAction("Contact");
            }
            return View();
        }
    }
}