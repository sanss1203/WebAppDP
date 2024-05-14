using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using WebAppDP.Models;
using Microsoft.AspNet.Identity;

namespace WebAppDP.Controllers
{
    public class ServicesController : Controller
    {

        private readonly db_digitalprintContext _context;

        public ServicesController()
        {
            _context = new db_digitalprintContext();
        }
        // GET: Services
        public ActionResult Details(int id = 1)
        {
            var service = new ProductModel { Id = id, Name = "Service" + id };
            var userID = User.Identity.GetUserName();

            var model = new ServicesViewModel();

            var userName = _context.Users
                .Where(u => u.Username == userID)
                .Select(u => u.NamaLengkap)
                .FirstOrDefault();

            string jnisService = "";
            string viewName = "";
            switch (id)
            {
                case 1:
                    jnisService = "InstallasiOS";
                    model.Nama_Lengkap = userName;
                    viewName = "InstallasiOS";
                    break;

                case 2:
                    jnisService = "MaintenanceHS";
                    model.Nama_Lengkap = userName;
                    viewName = "MaintenanceHS";
                    break;

                case 3:
                    jnisService = "ServiceKom";
                    model.Nama_Lengkap = userName;
                    viewName = "ServiceKom";
                    break;

                case 4:
                    jnisService = "RakitKomputer";
                    model.Nama_Lengkap = userName;
                    viewName = "RakitKomputer";
                    break;

                case 5:
                    jnisService = "BackupData";
                    model.Nama_Lengkap = userName;
                    viewName = "BackupData";
                    break;
            }

            model.Jnis_service = jnisService;
            return View(viewName, model);


        }

        public ActionResult InstallasiOs(ServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userID = User.Identity.GetUserName();
                string defaultStatus = "Status Request";
                DateTime tglMasuk = DateTime.ParseExact(model.Tgl_masuk, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                ServicesViewModel serviceAct = new ServicesViewModel
                {
                    Nama_Lengkap = model.Nama_Lengkap,
                    No_hp = model.No_hp,
                    Jnis_service = model.Jnis_service,
                    Jnis_Os = model.Jnis_Os,
                    Tgl_masuk = model.Tgl_masuk,
                    Estimasi_selesai = tglMasuk.AddDays(2).ToString("yyyy-MM-dd"),
                    //Estimasi_selesai = model.Tgl_masuk.AddDays(2),
                    Keterangan = model.Keterangan,
                    Status = defaultStatus,
                    Username = userID
                };

                _context.Services.Add(serviceAct);
                _context.SaveChanges();

                return RedirectToAction("Details", "Product", new { id = 3 });
            }
            return View();
        }

        public ActionResult StatusRequest(ServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                DateTime tglMasuk = DateTime.ParseExact(model.Tgl_masuk, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var userId = User.Identity.GetUserName();
                string defaultStatus = "Status Request";
                var serviceAct = new ServicesViewModel
                {
                    Nama_Lengkap = model.Nama_Lengkap,
                    No_hp = model.No_hp,
                    Jnis_service = model.Jnis_service,
                    Tgl_masuk = model.Tgl_masuk,
                    Estimasi_selesai = tglMasuk.AddDays(3).ToString("yyyy-MM-dd"),
                    //Estimasi_selesai = model.Tgl_masuk.AddDays(3),
                    Keterangan = model.Keterangan,
                    Status = defaultStatus,
                    Username = userId
                };
                _context.Services.Add(serviceAct);
                _context.SaveChanges();

                return RedirectToAction("Details", "Product", new { id = 3 });
            }
            return View();
        }
    }
}