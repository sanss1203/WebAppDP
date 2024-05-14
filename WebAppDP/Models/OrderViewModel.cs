using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppDP.Models
{
    public class OrderViewModel
    {
        
        [Key]
        public int Id_pesanan { get; set; }
        [Required]
        public string NamaPemesan { get; set; }
        public string Alamat { get; set; }
        [Required]
        public string NoHP { get; set; }
        
        public string JenisProduct { get; set; }

        public string UkuranDesain { get; set; }

        public string TanggalPesanan { get; set; }


        public string FileDesain { get; set; }

        public int JumlahCopy { get; set; }

        [Required]
        public string Keterangan { get; set; }

        [Required]
        public string MPembayaran { get; set; }

        public string Username { get; set; }

        [NotMapped]
        public bool HasPayment { get; set; }

        [NotMapped]
        public SelectList AlamatList { get; set; }

        [NotMapped]
        public SelectList JenisProductHargaList { get; set; }

        //[NotMapped]
        //public string Namalengkap { get; set; }



    }
}