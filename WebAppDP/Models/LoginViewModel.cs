using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace WebAppDP.Models
{
    public class LoginViewModel
    {
        [Key]
        public int Id_users { get; set; }

        [Required]
        public string NamaLengkap { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        //NotMapped akan diabaikan ketika berhubungan dengan database
        [NotMapped]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string KonfirmasiPassword { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public string VerificationCode { get; set; }
        
        //public LoginViewModel()
        //{
        //    IsVerified = false;
        //    VerificationCode = "";
        //}
    }
}
