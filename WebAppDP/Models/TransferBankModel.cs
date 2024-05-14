using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebAppDP.Models
{
    public class TransferBankModel
    {
        [Key]
        public int Id_transfer { get; set; }

        [Required]
        public string Nama_bank { get; set; }

        [Required]
        public string No_rekening { get; set; }

        [Required]
        public string Nama_penerima { get; set; }
    }
}