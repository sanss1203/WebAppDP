using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppDP.Models
{
    public class PaymentViewModel
    {
        //Key diartikan sebagai Primary Key
        [Key]
        public int Id_pembayaran { get; set; }
        [Required]
        public int Id_pesanan { get; set; }
        [Required]
        public string NamaPemesan { get; set; }
        [Required]
        public string UkuranDesain { get; set; }
        public string JumlahHalaman { get; set; }
        public string MPembayaran { get; set; }

        public int JumlahCopy { get; set; }

        public string JumlahOngkir { get; set; }

        [Required]
        public string HargaPembayaran { get; set; }
        
        public string BuktiTF { get; set; }
       

    }
}