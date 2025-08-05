using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DernekTakipSistemi
{
    /// <summary>
    /// Üye işlemleri servisi
    /// </summary>
    public class UyeService
    {
        private readonly DatabaseHelper dbHelper;

        public UyeService()
        {
            dbHelper = new DatabaseHelper();
            CreateUyelerTableIfNotExists();
        }

        /// <summary>
        /// Üyeler tablosunu oluşturur (yoksa)
        /// </summary>
        private void CreateUyelerTableIfNotExists()
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();

                    string createTable = @"
                        CREATE TABLE Uyeler (
                            UyeID AUTOINCREMENT PRIMARY KEY,
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

        /// <summary>
        /// Tüm üyeleri getirir
        /// </summary>
        public List<Uye> TumUyeleriGetir()
        {
            List<Uye> uyeler = new List<Uye>();

            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Uyeler ORDER BY Ad, Soyad";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uyeler.Add(new Uye
                            {
                                UyeID = Convert.ToInt32(reader["UyeID"]),
                                TC = reader["TC"].ToString(),
                                Ad = reader["Ad"].ToString(),
                                Soyad = reader["Soyad"].ToString(),
                                Telefon = reader["Telefon"]?.ToString() ?? "",
                                Email = reader["Email"]?.ToString() ?? "",
                                Adres = reader["Adres"]?.ToString() ?? "",
                                UyelikTarihi = Convert.ToDateTime(reader["UyelikTarihi"]),
                                UyelikDurumu = reader["UyelikDurumu"].ToString(),
                                AidatBorcu = reader["AidatBorcu"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AidatBorcu"]),
                                SonOdemeTarihi = reader["SonOdemeTarihi"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["SonOdemeTarihi"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Üyeler getirilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return uyeler;
        }

        /// <summary>
        /// ID'ye göre üye getirir
        /// </summary>
        public Uye UyeGetir(int uyeID)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Uyeler WHERE UyeID = ?";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UyeID", uyeID);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Uye
                                {
                                    UyeID = Convert.ToInt32(reader["UyeID"]),
                                    TC = reader["TC"].ToString(),
                                    Ad = reader["Ad"].ToString(),
                                    Soyad = reader["Soyad"].ToString(),
                                    Telefon = reader["Telefon"]?.ToString() ?? "",
                                    Email = reader["Email"]?.ToString() ?? "",
                                    Adres = reader["Adres"]?.ToString() ?? "",
                                    UyelikTarihi = Convert.ToDateTime(reader["UyelikTarihi"]),
                                    UyelikDurumu = reader["UyelikDurumu"].ToString(),
                                    AidatBorcu = reader["AidatBorcu"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AidatBorcu"]),
                                    SonOdemeTarihi = reader["SonOdemeTarihi"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["SonOdemeTarihi"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Üye getirilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    UyeID = Convert.ToInt32(reader["UyeID"]),
                                    TC = reader["TC"].ToString(),
                                    Ad = reader["Ad"].ToString(),
                                    Soyad = reader["Soyad"].ToString(),
                                    Telefon = reader["Telefon"]?.ToString() ?? "",
                                    Email = reader["Email"]?.ToString() ?? "",
                                    Adres = reader["Adres"]?.ToString() ?? "",
                                    UyelikTarihi = Convert.ToDateTime(reader["UyelikTarihi"]),
                                    UyelikDurumu = reader["UyelikDurumu"].ToString(),
                                    AidatBorcu = reader["AidatBorcu"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AidatBorcu"]),
                                    SonOdemeTarihi = reader["SonOdemeTarihi"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["SonOdemeTarihi"])
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
    }
}