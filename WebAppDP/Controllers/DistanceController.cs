using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;
using System.Text;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAppDP.Models;
using MailKit.Search;

namespace WebAppDP.Controllers
{
    public class DistanceController : Controller
    {
        // GET: Distance
        private readonly db_digitalprintContext _context;


        public DistanceController()
        {
            _context = new db_digitalprintContext();
        }

        [Authorize]
        public ActionResult Alamat()
        {
            var userID = User.Identity.GetUserName();
            
            //Tampilkan data alamat di Views berdasarkan userID
            var alamatList = _context.Alamat
                .Where(a => a.Username == userID)
                .OrderByDescending(a => a.Id_ongkir)
                .Select(a => new AlamatViewModel
                {
                    Id_ongkir = a.Id_ongkir,
                    Username = a.Username,
                    AlamatDetail = a.AlamatDetail,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    HargaOngkir = a.HargaOngkir
                })
                .ToList();

            return View(alamatList);
            //return View();
        }

        [Authorize]
        public ActionResult HapusAlamat(int id)
        {
            if (ModelState.IsValid)
            {
                var ongkir = _context.Alamat
                    .Where(a => a.Id_ongkir == id)
                    .FirstOrDefault();

                if (ongkir == null)
                {
                    return HttpNotFound();
                }

                _context.Alamat.Remove(ongkir);
                _context.SaveChanges();

                return RedirectToAction("Alamat");
            }
            return View();
        }



        public async Task<ActionResult> CekOngkir(int id)
        {
            var userID = User.Identity.GetUserName();
            var alamatEntity = _context.Alamat
                .Where(a => a.Username == userID)
                .FirstOrDefault(a => a.Id_ongkir == id);

            // Melakukan perhitungan ongkir berdasarkan item yang dipilih
            double distance = 0;
            if (alamatEntity != null)
            {
                string endLocation = $"{alamatEntity.Longitude},{alamatEntity.Latitude}";
                string apiUrl = $"https://us1.locationiq.com/v1/directions/driving/{Uri.EscapeDataString("124.8765224,1.2779935")};{Uri.EscapeDataString(endLocation)}?key=pk.4909e165f706910421235ac09dea969d&overview=full";

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage distanceResponse = await httpClient.GetAsync(apiUrl);
                    distanceResponse.EnsureSuccessStatusCode();
                    string distanceResponseContent = await distanceResponse.Content.ReadAsStringAsync();

                    JObject distanceData = JObject.Parse(distanceResponseContent);
                    distance = (double)distanceData["routes"][0]["distance"];
                }
            }

            double ongkir = distance < 500 ? 1000 : distance / 500 * 2000;

            // Mengupdate harga ongkir pada item Alamat
            if (alamatEntity != null)
            {
                alamatEntity.HargaOngkir = ongkir.ToString();
                _context.SaveChanges();
            }

            // Redirect atau tampilkan hasil perhitungan ongkir

            return RedirectToAction("Alamat");
        }

        public async Task<ActionResult> CekDistance()
        {
            string apiUrl = $"https://us1.locationiq.com/v1/directions/driving/124.8765224,1.2779935;124.916353,1.308576?key=pk.4909e165f706910421235ac09dea969d&overview=full";

            using (HttpClient httpClient = new HttpClient())
            {
                //Buat Http Request ke API 
                HttpResponseMessage distanceResponse = await httpClient.GetAsync(apiUrl);
                distanceResponse.EnsureSuccessStatusCode();
                string distanceResponseContent = await distanceResponse.Content.ReadAsStringAsync();

                //Ambil jarak antara dua lokasi dari Response Json
                JObject distanceData = JObject.Parse(distanceResponseContent);
                double distance = (double)distanceData["routes"][0]["distance"];


                ViewBag.Distances = distance;
                return View();
            }
        }

        //public ActionResult simpanAlamat(string alamatDetail, double latitude, double longitude, string hasilOngkir)
        //{
        //    var userID = User.Identity.GetUserName();

        //    var alamatEntity = new AlamatViewModel
        //    {
        //        AlamatDetail = alamatDetail,
        //        Username = userID,
        //        Latitude = latitude,
        //        Longitude = longitude,
        //        HargaOngkir = hasilOngkir
        //    };
        //    //Save semua data ke database
        //    _context.Alamat.Add(alamatEntity);
        //    _context.SaveChanges();

        //    return RedirectToAction("Details", "Product", new { id = 1 });
        //}


        //Kode untuk menghitung ongkir sekaligus menyimpan alamat
        //public async Task<ActionResult> DistanceAction(string alamatDetail, double latitude, double longitude)
        //{

        //        string endLocation = $"{longitude},{latitude}";

        //        string apiUrl = $"https://us1.locationiq.com/v1/directions/driving/124.8765224,1.2779935;{endLocation}?key=pk.5dd685a490e1e137162422a485dcee54&overview=full";
        //        HttpClient httpClient = new HttpClient();

        //        HttpResponseMessage distanceResponse = await httpClient.GetAsync(apiUrl);
        //        distanceResponse.EnsureSuccessStatusCode();
        //        string distanceResponseContent = await distanceResponse.Content.ReadAsStringAsync();

        //        JObject distanceData = JObject.Parse(distanceResponseContent);
        //        double distance = (double)distanceData["routes"][0]["distance"];

        //        double ongkir = distance < 500 ? 1000 : distance / 500 * 2000;

        //        string finalHar = ongkir.ToString();

        //        TempData["TtlOngkr"] = finalHar;
        //        TempData["Lat"] = latitude;
        //        TempData["Lon"] = longitude;
        //        TempData["alamtDetail"] = alamatDetail;

        //        return RedirectToAction("LihatOngkir");


        //}

    }
}