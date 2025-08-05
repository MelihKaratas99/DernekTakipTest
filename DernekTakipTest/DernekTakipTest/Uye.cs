using System;

namespace DernekTakipSistemi
{
    /// <summary>
    /// Üye model sınıfı
    /// </summary>
    public class Uye
    {
        public int UyeID { get; set; }
        public string TC { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public DateTime UyelikTarihi { get; set; }
        public string UyelikDurumu { get; set; } // "Aktif", "Pasif"
        public decimal AidatBorcu { get; set; }
        public DateTime? SonOdemeTarihi { get; set; }

        public Uye()
        {
            UyelikTarihi = DateTime.Now;
            UyelikDurumu = "Aktif";
            AidatBorcu = 0;
        }

        public string AdSoyad => $"{Ad} {Soyad}";

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(TC) &&
                   !string.IsNullOrWhiteSpace(Ad) &&
                   !string.IsNullOrWhiteSpace(Soyad) &&
                   !string.IsNullOrWhiteSpace(Telefon);
        }

        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(TC))
                return "TC Kimlik No boş olamaz";
            if (string.IsNullOrWhiteSpace(Ad))
                return "Ad boş olamaz";
            if (string.IsNullOrWhiteSpace(Soyad))
                return "Soyad boş olamaz";
            if (string.IsNullOrWhiteSpace(Telefon))
                return "Telefon boş olamaz";
            return string.Empty;
        }
    }
}