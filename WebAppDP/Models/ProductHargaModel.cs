using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebAppDP.Models
{
    public class ProductHargaModel
    {
        [Key]
        public int Id_harga { get; set; }

        [Required]
        public string JenisProduct { get; set; }

        [Required]
        public string HargaProduct { get; set; }
    }
}
