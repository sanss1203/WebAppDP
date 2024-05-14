using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppDP.Models
{
    public class ServicesViewModel
    {
        [Key]
        public int Id_services { get; set; }
        [Required]
        public string Nama_Lengkap { get; set; }
        [Required]
        public string No_hp { get; set; }
        [Required]
        public string Jnis_service { get; set; }
        public string Jnis_Os { get; set; }
        [Required]
        public string Tgl_masuk { get; set; }
        public string Estimasi_selesai { get; set; }
        public string Keterangan { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
    }
}