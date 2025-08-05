using System;

namespace DernekTakipSistemi
{
    /// <summary>
    /// Kullanıcı rolleri enum
    /// </summary>
    public enum UserRole
    {
        Uye = 1,        // Üye
        Yonetici = 2    // Yönetici
    }

    /// <summary>
    /// Kullanıcı model sınıfı
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? SonGirisTarihi { get; set; }

        // Üye ile ilişki (eğer rol Üye ise)
        public int? UyeID { get; set; }

        public User()
        {
            IsActive = true;
            KayitTarihi = DateTime.Now;
            Role = UserRole.Uye;
        }

        public string AdSoyad => $"{Ad} {Soyad}";
        public string RoleText => Role == UserRole.Yonetici ? "Yönetici" : "Üye";

        /// <summary>
        /// Kullanıcının yönetici olup olmadığını kontrol eder
        /// </summary>
        public bool IsAdmin => Role == UserRole.Yonetici;

        /// <summary>
        /// Kullanıcının üye olup olmadığını kontrol eder
        /// </summary>
        public bool IsMember => Role == UserRole.Uye;

        /// <summary>
        /// Şifre doğrulama (basit)
        /// </summary>
        public bool VerifyPassword(string password)
        {
            // Gerçek projede hash kullanılmalı
            return this.Sifre == password;
        }

        /// <summary>
        /// Şifre hash'leme (basit - gerçek projede bcrypt kullanın)
        /// </summary>
        public static string HashPassword(string password)
        {
            // Basit hash - gerçek projede BCrypt kullanın
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password + "SALT"));
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(KullaniciAdi) &&
                   !string.IsNullOrWhiteSpace(Sifre) &&
                   !string.IsNullOrWhiteSpace(Ad) &&
                   !string.IsNullOrWhiteSpace(Soyad);
        }

        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(KullaniciAdi))
                return "Kullanıcı adı boş olamaz";

            if (string.IsNullOrWhiteSpace(Sifre))
                return "Şifre boş olamaz";

            if (string.IsNullOrWhiteSpace(Ad))
                return "Ad boş olamaz";

            if (string.IsNullOrWhiteSpace(Soyad))
                return "Soyad boş olamaz";

            return string.Empty;
        }
    }

    /// <summary>
    /// Mevcut giriş yapmış kullanıcı bilgilerini tutan static sınıf
    /// </summary>
    public static class CurrentUser
    {
        public static User User { get; private set; }
        public static bool IsLoggedIn => User != null;
        public static bool IsAdmin => User?.IsAdmin ?? false;
        public static bool IsMember => User?.IsMember ?? false;

        public static void Login(User user)
        {
            User = user;
            User.SonGirisTarihi = DateTime.Now;
        }

        public static void Logout()
        {
            User = null;
        }

        public static string GetWelcomeMessage()
        {
            if (!IsLoggedIn) return "Misafir";
            return $"Hoş geldin, {User.Ad} {User.Soyad} ({User.RoleText})";
        }
    }
}