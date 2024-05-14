using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppDP.Models
{
    //Model ini merupakan gabungnan dari Model OrderPaymentViewModel yang hanya berfungsi untuk menampilkan data
    public class OrderPaymentViewModel
    {
        public int Id_pesanan { get; set; }
        public string NamaPemesan { get; set; }
        public string NoHP { get; set; }
        public string TanggalPesanan { get; set; }
        public string JenisProduct { get; set; }
        public string UkuranDesain { get; set; }
        public string FileDesain { get; set; }
        public string MPembayaran { get; set; }
        public bool HasPayment { get; set; }
        public int Id_pembayaran { get; set; }
        public string HargaPembayaran { get; set; }
        public string BuktiTF { get; set; }
    }
}