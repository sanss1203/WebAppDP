using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppDP.Models
{
    public class ContactViewModel
    {
        [Key]
        public int Id_contact { get; set; }
        [Required]
        public string NamaPengirim { get; set; }
        [Required]
        public string EmailPengirim { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Pesan { get; set; }
    }
}