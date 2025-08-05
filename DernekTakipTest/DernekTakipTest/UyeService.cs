using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DernekTakipSistemi
{
    /// <summary>
    /// Üye işlemleri servisi (Access 2001 uyumlu - Helper metodları ile)
    /// </summary>
    public class UyeService
    {
        private readonly DatabaseHelper dbHelper;

        public UyeService()
        {
            dbHelper = new DatabaseHelper();
            CreateUyelerTableIfNotExists();
        }

        #region Helper Metodları

        /// <summary>
        /// DataReader'dan güvenli DateTime okur
        /// </summary>
        private DateTime SafeGetDateTime(OleDbDataReader reader, string columnName, DateTime defaultValue = default)
        {
            try
            {
                if (reader[columnName] == DBNull.Value || reader[columnName] == null)
                {
                    return defaultValue == default ? DateTime.Now : defaultValue;
                }
                return Convert.ToDateTime(reader[columnName]);
            }
            catch
            {
                return defaultValue == default ? DateTime.Now : defaultValue;
            }
        }

        /// <summary>
        /// DataReader'dan güvenli nullable DateTime okur
        /// </summary>
        private DateTime? SafeGetNullableDateTime(OleDbDataReader reader, string columnName)
        {
            try
            {
                if (reader[columnName] == DBNull.Value || reader[columnName] == null)
                {
                    return null;
                }
                return Convert.ToDateTime(reader[columnName]);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// DataReader'dan güvenli string okur
        /// </summary>
        private string SafeGetString(OleDbDataReader reader, string columnName, string defaultValue = "")
        {
            try
            {
                return reader[columnName]?.ToString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// DataReader'dan güvenli decimal okur
        /// </summary>
        private decimal SafeGetDecimal(OleDbDataReader reader, string columnName, decimal defaultValue = 0)
        {
            try
            {
                if (reader[columnName] == DBNull.Value)
                    return defaultValue;
                return Convert.ToDecimal(reader[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// DataReader'dan güvenli int okur
        /// </summary>
        private int SafeGetInt(OleDbDataReader reader, string columnName, int defaultValue = 0)
        {
            try
            {
                if (reader[columnName] == DBNull.Value)
                    return defaultValue;
                return Convert.ToInt32(reader[columnName]);
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion

        #region Tablo Oluşturma

        /// <summary>
        /// Üyeler tablosunu oluşturur (yoksa) - Access 2001 uyumlu
        /// </summary>
        private void CreateUyelerTableIfNotExists()
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();

                    // Access 2001 için uyumlu tablo oluşturma
                    string createTable = @"
                        CREATE TABLE Uyeler (
                            UyeID COUNTER PRIMARY KEY,
                            TC TEXT(11) NOT NULL,
                            Ad TEXT(50) NOT NULL,
                            Soyad TEXT(50) NOT NULL,
                            Telefon TEXT(15),
                            Email TEXT(100),
                            Adres MEMO,
                            UyelikTarihi DATETIME DEFAULT Now(),
                            UyelikDurumu TEXT(10) DEFAULT 'Aktif',
                            AidatBorcu CURRENCY DEFAULT 0,
                            SonOdemeTarihi DATETIME
                        )";

                    using (var command = new OleDbCommand(createTable, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            // Tablo zaten varsa hata vermez
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Üyeler tablosu oluşturulurken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Üye Getirme İşlemleri

        /// <summary>
        /// Tüm üyeleri getirir
        /// </summary>
        public List<Uye> TumUyeleriGetir()
        {
            List<Uye> uyeler = new List<Uye>();

            try
            {
                System.Diagnostics.Debug.WriteLine("TumUyeleriGetir başladı...");

                if (dbHelper == null)
                {
                    System.Diagnostics.Debug.WriteLine("dbHelper null!");
                    MessageBox.Show("Veritabanı bağlantısı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return uyeler;
                }

                using (var connection = dbHelper.GetConnection())
                {
                    if (connection == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Connection null!");
                        return uyeler;
                    }

                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Veritabanı bağlantısı açıldı.");

                    string query = "SELECT * FROM Uyeler ORDER BY Ad, Soyad";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        System.Diagnostics.Debug.WriteLine("Query çalıştırıldı.");

                        int rowCount = 0;
                        while (reader.Read())
                        {
                            rowCount++;
                            try
                            {
                                var uye = new Uye
                                {
                                    UyeID = SafeGetInt(reader, "UyeID"),
                                    TC = SafeGetString(reader, "TC"),
                                    Ad = SafeGetString(reader, "Ad"),
                                    Soyad = SafeGetString(reader, "Soyad"),
                                    Telefon = SafeGetString(reader, "Telefon"),
                                    Email = SafeGetString(reader, "Email"),
                                    Adres = SafeGetString(reader, "Adres"),
                                    UyelikTarihi = SafeGetDateTime(reader, "UyelikTarihi"),
                                    UyelikDurumu = SafeGetString(reader, "UyelikDurumu", "Aktif"),
                                    AidatBorcu = SafeGetDecimal(reader, "AidatBorcu"),
                                    SonOdemeTarihi = SafeGetNullableDateTime(reader, "SonOdemeTarihi")
                                };

                                uyeler.Add(uye);
                                System.Diagnostics.Debug.WriteLine($"Üye eklendi: {uye.AdSoyad}");
                            }
                            catch (Exception rowEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Satır {rowCount} okuma hatası: {rowEx.Message}");
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"Toplam {uyeler.Count} üye okundu.");
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"Üyeler getirilirken hata: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"{errorMsg}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show(errorMsg, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return uyeler;
        }

        /// <summary>
        /// ID'ye göre üye getirir
        /// </summary>
        /// <summary>
        /// ID'ye göre üye getirir - Debug versiyonu
        /// </summary>
        public Uye UyeGetir(int uyeID)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"UyeGetir başladı - Aranan UyeID: {uyeID}");

                if (dbHelper == null)
                {
                    System.Diagnostics.Debug.WriteLine("dbHelper null!");
                    return null;
                }

                using (var connection = dbHelper.GetConnection())
                {
                    if (connection == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Connection null!");
                        return null;
                    }

                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Veritabanı bağlantısı açıldı.");

                    string query = "SELECT * FROM Uyeler WHERE UyeID = ?";
                    System.Diagnostics.Debug.WriteLine($"SQL Query: {query}");

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.Add("@UyeID", OleDbType.Integer).Value = uyeID;
                        System.Diagnostics.Debug.WriteLine($"Parametre eklendi: UyeID = {uyeID}");

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine("Üye bulundu! Veri okunuyor...");

                                var uye = new Uye
                                {
                                    UyeID = SafeGetInt(reader, "UyeID"),
                                    TC = SafeGetString(reader, "TC"),
                                    Ad = SafeGetString(reader, "Ad"),
                                    Soyad = SafeGetString(reader, "Soyad"),
                                    Telefon = SafeGetString(reader, "Telefon"),
                                    Email = SafeGetString(reader, "Email"),
                                    Adres = SafeGetString(reader, "Adres"),
                                    UyelikTarihi = SafeGetDateTime(reader, "UyelikTarihi"),
                                    UyelikDurumu = SafeGetString(reader, "UyelikDurumu", "Aktif"),
                                    AidatBorcu = SafeGetDecimal(reader, "AidatBorcu"),
                                    SonOdemeTarihi = SafeGetNullableDateTime(reader, "SonOdemeTarihi")
                                };

                                System.Diagnostics.Debug.WriteLine($"Üye başarıyla okundu: {uye.AdSoyad} (ID: {uye.UyeID})");
                                return uye;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"UyeID {uyeID} ile eşleşen üye bulunamadı!");

                                // Tüm üyeleri listele (debug için)
                                connection.Close();
                                connection.Open();

                                using (var allCommand = new OleDbCommand("SELECT UyeID, Ad, Soyad FROM Uyeler", connection))
                                using (var allReader = allCommand.ExecuteReader())
                                {
                                    System.Diagnostics.Debug.WriteLine("=== Mevcut tüm üyeler ===");
                                    while (allReader.Read())
                                    {
                                        int id = SafeGetInt(allReader, "UyeID");
                                        string ad = SafeGetString(allReader, "Ad");
                                        string soyad = SafeGetString(allReader, "Soyad");
                                        System.Diagnostics.Debug.WriteLine($"UyeID: {id}, Ad: {ad}, Soyad: {soyad}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"Üye getirilirken hata: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"{errorMsg}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show(errorMsg, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        /// <summary>
        /// Üye arama
        /// </summary>
        public List<Uye> UyeAra(string aramaMetni)
        {
            List<Uye> sonuclar = new List<Uye>();

            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT * FROM Uyeler 
                        WHERE TC LIKE ? OR Ad LIKE ? OR Soyad LIKE ? OR Telefon LIKE ? OR Email LIKE ?
                        ORDER BY Ad, Soyad";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        string searchParam = $"%{aramaMetni}%";
                        command.Parameters.AddWithValue("@TC", searchParam);
                        command.Parameters.AddWithValue("@Ad", searchParam);
                        command.Parameters.AddWithValue("@Soyad", searchParam);
                        command.Parameters.AddWithValue("@Telefon", searchParam);
                        command.Parameters.AddWithValue("@Email", searchParam);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sonuclar.Add(new Uye
                                {
                                    UyeID = SafeGetInt(reader, "UyeID"),
                                    TC = SafeGetString(reader, "TC"),
                                    Ad = SafeGetString(reader, "Ad"),
                                    Soyad = SafeGetString(reader, "Soyad"),
                                    Telefon = SafeGetString(reader, "Telefon"),
                                    Email = SafeGetString(reader, "Email"),
                                    Adres = SafeGetString(reader, "Adres"),
                                    UyelikTarihi = SafeGetDateTime(reader, "UyelikTarihi"),
                                    UyelikDurumu = SafeGetString(reader, "UyelikDurumu", "Aktif"),
                                    AidatBorcu = SafeGetDecimal(reader, "AidatBorcu"),
                                    SonOdemeTarihi = SafeGetNullableDateTime(reader, "SonOdemeTarihi")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Arama sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return sonuclar;
        }

        #endregion

        #region Üye Ekleme/Güncelleme/Silme

        /// <summary>
        /// Yeni üye ekler - Access 2001 uyumlu
        /// </summary>
        public bool UyeEkle(Uye uye)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();

                    // Access için daha basit sorgu - varsayılan değerler veritabanında ayarlanmış
                    string query = @"
                INSERT INTO Uyeler (TC, Ad, Soyad, Telefon, Email, Adres, UyelikTarihi, UyelikDurumu, AidatBorcu)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        // Parametreleri tek tek ekle ve veri türünü kontrol et
                        command.Parameters.Add("@TC", OleDbType.VarChar, 11).Value = uye.TC ?? "";
                        command.Parameters.Add("@Ad", OleDbType.VarChar, 50).Value = uye.Ad ?? "";
                        command.Parameters.Add("@Soyad", OleDbType.VarChar, 50).Value = uye.Soyad ?? "";
                        command.Parameters.Add("@Telefon", OleDbType.VarChar, 15).Value = uye.Telefon ?? "";
                        command.Parameters.Add("@Email", OleDbType.VarChar, 100).Value = uye.Email ?? "";
                        command.Parameters.Add("@Adres", OleDbType.LongVarChar).Value = uye.Adres ?? "";
                        command.Parameters.Add("@UyelikTarihi", OleDbType.Date).Value = uye.UyelikTarihi;
                        command.Parameters.Add("@UyelikDurumu", OleDbType.VarChar, 10).Value = uye.UyelikDurumu ?? "Aktif";
                        command.Parameters.Add("@AidatBorcu", OleDbType.Currency).Value = uye.AidatBorcu;

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Üye eklenirken hata: {ex.Message}\n\nSQL Detay: Veri türü uyumsuzluğu olabilir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"UyeEkle Hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Üye günceller - Access 2001 uyumlu
        /// </summary>
        public bool UyeGuncelle(Uye uye)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                UPDATE Uyeler SET TC=?, Ad=?, Soyad=?, Telefon=?, Email=?, Adres=?, UyelikTarihi=?, UyelikDurumu=?
                WHERE UyeID=?";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        // Parametreleri veri türü ile birlikte ekle
                        command.Parameters.Add("@TC", OleDbType.VarChar, 11).Value = uye.TC ?? "";
                        command.Parameters.Add("@Ad", OleDbType.VarChar, 50).Value = uye.Ad ?? "";
                        command.Parameters.Add("@Soyad", OleDbType.VarChar, 50).Value = uye.Soyad ?? "";
                        command.Parameters.Add("@Telefon", OleDbType.VarChar, 15).Value = uye.Telefon ?? "";
                        command.Parameters.Add("@Email", OleDbType.VarChar, 100).Value = uye.Email ?? "";
                        command.Parameters.Add("@Adres", OleDbType.LongVarChar).Value = uye.Adres ?? "";
                        command.Parameters.Add("@UyelikTarihi", OleDbType.Date).Value = uye.UyelikTarihi;
                        command.Parameters.Add("@UyelikDurumu", OleDbType.VarChar, 10).Value = uye.UyelikDurumu ?? "Aktif";
                        command.Parameters.Add("@UyeID", OleDbType.Integer).Value = uye.UyeID;

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Üye güncellenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"UyeGuncelle Hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Üye siler
        /// </summary>
        public bool UyeSil(int uyeID)
        {
            try
            {
                DialogResult result = MessageBox.Show("Bu üyeyi silmek istediğinizden emin misiniz?",
                    "Üye Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    using (var connection = dbHelper.GetConnection())
                    {
                        connection.Open();
                        string query = "DELETE FROM Uyeler WHERE UyeID = ?";

                        using (var command = new OleDbCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UyeID", uyeID);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Üye başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Üye silinirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        #endregion

        #region Yardımcı Metodlar

        /// <summary>
        /// Toplam üye sayısını getirir
        /// </summary>
        public int ToplamUyeSayisi()
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Uyeler";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Aktif üye sayısını getirir
        /// </summary>
        public int AktifUyeSayisi()
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Uyeler WHERE UyelikDurumu = 'Aktif'";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Borçlu üye sayısını getirir
        /// </summary>
        public int BorcluUyeSayisi()
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Uyeler WHERE AidatBorcu > 0";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// TC kimlik no'ya göre üye var mı kontrol eder
        /// </summary>
        public bool TCVarMi(string tc, int excludeUyeID = 0)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = excludeUyeID > 0
                        ? "SELECT COUNT(*) FROM Uyeler WHERE TC = ? AND UyeID <> ?"
                        : "SELECT COUNT(*) FROM Uyeler WHERE TC = ?";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TC", tc);
                        if (excludeUyeID > 0)
                            command.Parameters.AddWithValue("@UyeID", excludeUyeID);

                        object result = command.ExecuteScalar();
                        int count = result != null ? Convert.ToInt32(result) : 0;
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}