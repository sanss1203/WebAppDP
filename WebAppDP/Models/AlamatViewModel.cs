using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppDP.Models
{
    public class AlamatViewModel
    {
        [Key]
        public int Id_ongkir { get; set; }

        public string Username { get; set; }

        [Required]
        public string AlamatDetail { get; set; }

        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public string HargaOngkir { get; set; }
    }
}