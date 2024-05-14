using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MySql.Data.EntityFrameworkCore;


// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebAppDP.Models
{
    public partial class db_digitalprintContext : DbContext
    {
        public db_digitalprintContext()
        {
        }

        public db_digitalprintContext(DbContextOptions<db_digitalprintContext> options)
            : base(options)
        {
        }

        //Deklarasi DbSet untuk mengakses entitas pada setiap Models 
        public virtual DbSet<LoginViewModel> Users { get; set; }
        public virtual DbSet<OrderViewModel> Order { get; set; }
        public virtual DbSet<PaymentViewModel> Payment { get; set; }
        public virtual DbSet<AdminLoginModel> Admin { get; set; }
        public virtual DbSet<ContactViewModel> Messages { get; set; }
        public virtual DbSet<AlamatViewModel> Alamat { get; set; }
        public virtual DbSet<ServicesViewModel> Services { get; set; }
        public virtual DbSet<TransferBankModel> TransferBank { get; set; }

        public virtual DbSet<ProductHargaModel> ProductHarga { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("Server=localhost;Database=db_digitalprint;Uid=root;Pwd=");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Buat Chaining Method untuk mengkonfigurasi pemetaan antara entitas LoginViewModel dan tabel di database.
            //Pada kasus ini Model LoginViewModel dan tabel users pada database
            modelBuilder.Entity<LoginViewModel>(entity =>
            {
                
                entity.ToTable("users");

                entity.Property(e => e.Id_users).HasColumnName("Id_users");

                entity.Property(e => e.NamaLengkap)
                    .HasColumnName("namaLengkap")
                    .HasMaxLength(100);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(100);


                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(100);

                entity.Property(e => e.IsVerified)
                    .HasColumnName("IsVerified");

                entity.Property(e => e.VerificationCode)
                    .HasColumnName("VerificationCode");
            });

            //Buat Chaining Method untuk mengkonfigurasi pemetaan antara entitas model AdminLoginModel dan tabel di database.
            //pada kasus ini Model AdminLoginModel dan tabel tb_admin pada database

            modelBuilder.Entity<AdminLoginModel>(entity =>
            {

                entity.ToTable("tb_admin");

                entity.Property(e => e.Id_admin)
                    .HasColumnName("Id_admin");

                entity.Property(e => e.Username)
                    .HasColumnName("Username")
                    .HasMaxLength(150);

                entity.Property(e => e.Password)
                    .HasColumnName("Password")
                    .HasMaxLength(150);

                entity.Property(e => e.Role)
                    .HasColumnName("Role")
                    .HasMaxLength(100);
            });

            //Buat Chaining Method untuk mengkonfigurasi pemetaan antara entitas OrderViewModel dan tabel di database.
            //Pada kasus ini Model OrderViewModel dan tabel tb_order pada database
            modelBuilder.Entity<OrderViewModel>(entity =>
            {
                entity.ToTable("tb_order");

                entity.Property(e => e.Id_pesanan)
                    .HasColumnName("Id_pesanan")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NamaPemesan)
                    .IsRequired()
                    .HasColumnName("NamaPemesan")
                    .HasMaxLength(255);
                    

                entity.Property(e => e.Alamat)
                    .IsRequired()
                    .HasColumnName("Alamat")
                    .HasMaxLength(255);

                entity.Property(e => e.NoHP)
                    .IsRequired()
                    .HasColumnName("NoHP")
                    .HasMaxLength(45);

                entity.Property(e => e.JenisProduct)
                    .IsRequired()
                    .HasColumnName("JenisProduct")
                    .HasMaxLength(100);

                entity.Property(e => e.UkuranDesain)
                    .HasColumnName("UkuranDesain")
                    .HasMaxLength(45);

                entity.Property(e => e.TanggalPesanan)
                    .HasColumnName("TanggalPesanan");
                    

                entity.Property(e => e.FileDesain)
                    .HasColumnName("FileDesain")
                    .HasMaxLength(255);

                entity.Property(e => e.JumlahCopy)
                    .HasColumnName("JumlahCopy");

                entity.Property(e => e.Keterangan)
                    .HasColumnName("Keterangan")
                    .HasMaxLength(255);

                entity.Property(e => e.MPembayaran)
                    .IsRequired()
                    .HasColumnName("MPembayaran")
                    .HasMaxLength(100);

                entity.Property(e => e.Username)
                    .HasColumnName("username");

            });

            //Buat Chaining Method untuk mengkonfigurasi pemetaan antara entitas PaymentViewModel dan tabel di database.
            //Pada kasus ini Model PaymentViewModel dan tabel tb_pembayaran pada database;
            modelBuilder.Entity<PaymentViewModel>(entity =>
            {
                entity.ToTable("tb_pembayaran");

                entity.Property(e => e.Id_pembayaran)
                    .HasColumnName("Id_pembayaran")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Id_pesanan)
                    .HasColumnName("Id_pesanan");

                entity.Property(e => e.NamaPemesan)
                    .HasColumnName("NamaPemesan")
                    .HasMaxLength(255);

                entity.Property(e => e.UkuranDesain)
                    .HasColumnName("UkuranDesain")
                    .HasMaxLength(100);

                entity.Property(e => e.JumlahHalaman)
                    .HasColumnName("JumlahHalaman")
                    .HasMaxLength(100);

                entity.Property(e => e.MPembayaran)
                    .HasColumnName("MPembayaran");

                entity.Property(e => e.JumlahCopy)
                    .HasColumnName("JumlahCopy");

                entity.Property(e => e.JumlahOngkir)
                    .HasColumnName("JumlahOngkir")
                    .HasMaxLength(100);

                entity.Property(e => e.HargaPembayaran)
                    .HasColumnName("HargaPembayaran")
                    .HasMaxLength(100);

                entity.Property(e => e.BuktiTF)
                    .HasColumnName("BuktiTF")
                    .HasMaxLength(100);

            });

            //Buat Chaining Method untuk mengkonfigurasi pemetaan antara entitas LoginViewModel dan tabel di database.
            //Pada kasus ini Model ContactViewModel dan tabel tb_contact pada database;
            modelBuilder.Entity<ContactViewModel>(entity =>
            {
                entity.ToTable("tb_contact");

                entity.Property(e => e.NamaPengirim)
                    .HasColumnName("NamaPengirim")
                    .HasMaxLength(100);

                entity.Property(e => e.EmailPengirim)
                    .HasColumnName("EmailPengirim")
                    .HasMaxLength(100);

                entity.Property(e => e.Subject)
                    .HasColumnName("Subject")
                    .HasMaxLength(100);

                entity.Property(e => e.Pesan)
                    .HasColumnName("Pesan")
                    .HasMaxLength(255);


            });

            modelBuilder.Entity<AlamatViewModel>(entity =>
            {
                entity.ToTable("tb_ongkir");

                entity.Property(e => e.Id_ongkir)
                    .HasColumnName("id_ongkir");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(45);

                entity.Property(e => e.AlamatDetail)
                    .HasColumnName("detail_Alamat")
                    .HasMaxLength(100);

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude");

                entity.Property(e => e.HargaOngkir)
                    .HasColumnName("harga_ongkir")
                    .HasMaxLength(100);

            });

            modelBuilder.Entity<ServicesViewModel>(entity =>
            {
                entity.ToTable("tb_services");

                entity.Property(e => e.Id_services)
                    .HasColumnName("Id_Services");

                entity.Property(e => e.Nama_Lengkap)
                    .HasColumnName("Nama_Lengkap")
                    .HasMaxLength(100);

                entity.Property(e => e.No_hp)
                    .HasColumnName("No_Hp")
                    .HasMaxLength(100);

                entity.Property(e => e.Jnis_service)
                    .HasColumnName("Jnis_Service")
                    .HasMaxLength(100);

                entity.Property(e => e.Jnis_Os)
                    .HasColumnName("Jnis_OS")
                    .HasMaxLength(100);

                entity.Property(e => e.Tgl_masuk)
                    .HasColumnName("Tgl_Masuk");
                    

                entity.Property(e => e.Estimasi_selesai)
                    .HasColumnName("Estimasi_Selesai");

                entity.Property(e => e.Keterangan)
                    .HasColumnName("Keterangan")
                    .HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .HasMaxLength(100);

                entity.Property(e => e.Username)
                    .HasColumnName("Usernames")
                    .HasMaxLength(100);

            });

            modelBuilder.Entity<TransferBankModel>(entity =>
            {
                entity.ToTable("Tb_transfer");

                entity.Property(e => e.Id_transfer)
                    .HasColumnName("Id_TransferRek");

                entity.Property(e => e.Nama_bank)
                    .HasColumnName("Nama_Bank")
                    .HasMaxLength(100);

                entity.Property(e => e.No_rekening)
                    .HasColumnName("No_Rek")
                    .HasMaxLength(100);

                entity.Property(e => e.Nama_penerima)
                    .HasColumnName("Nama_Penerima")
                    .HasMaxLength(100);
            });


            modelBuilder.Entity<ProductHargaModel>(entity =>
            {
                entity.ToTable("tb_harga");

                entity.Property(e => e.Id_harga)
                    .HasColumnName("Id_harga");

                entity.Property(e => e.JenisProduct)
                    .HasColumnName("JenisProduct")
                    .HasMaxLength(100);

                entity.Property(e => e.HargaProduct)
                    .HasColumnName("HargaProduct")
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
