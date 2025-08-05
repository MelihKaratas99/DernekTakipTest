using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DernekTakipSistemi
{
    public class UserService
    {
        private readonly DatabaseHelper dbHelper;

        public UserService()
        {
            dbHelper = new DatabaseHelper();
            CreateUsersTableIfNotExists();
            CreateDefaultAdmin();
        }

        /// <summary>
        /// Kullanıcılar tablosunu oluşturur (yoksa)
        /// </summary>
        private void CreateUsersTableIfNotExists()
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();

                    string createTable = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            UserID AUTOINCREMENT PRIMARY KEY,
                            KullaniciAdi TEXT(50) NOT NULL,
                            Sifre TEXT(255) NOT NULL,
                            Ad TEXT(50) NOT NULL,
                            Soyad TEXT(50) NOT NULL,
                            Email TEXT(100),
                            Role INTEGER DEFAULT 1,
                            IsActive YESNO DEFAULT Yes,
                            KayitTarihi DATETIME DEFAULT Now(),
                            SonGirisTarihi DATETIME,
                            UyeID INTEGER
                        )";

                    using (var command = new OleDbCommand(createTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Users tablosu oluşturulurken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Varsayılan admin kullanıcısını oluşturur
        /// </summary>
        private void CreateDefaultAdmin()
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();

                    // Admin var mı kontrol et
                    string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Role = 2";
                    using (var command = new OleDbCommand(checkAdmin, connection))
                    {
                        int adminCount = (int)command.ExecuteScalar();

                        if (adminCount == 0)
                        {
                            // Varsayılan admin oluştur
                            string insertAdmin = @"
                                INSERT INTO Users (KullaniciAdi, Sifre, Ad, Soyad, Email, Role, IsActive, KayitTarihi)
                                VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                            using (var cmd = new OleDbCommand(insertAdmin, connection))
                            {
                                cmd.Parameters.AddWithValue("@KullaniciAdi", "admin");
                                cmd.Parameters.AddWithValue("@Sifre", "123456"); // Basit şifre
                                cmd.Parameters.AddWithValue("@Ad", "Sistem");
                                cmd.Parameters.AddWithValue("@Soyad", "Yöneticisi");
                                cmd.Parameters.AddWithValue("@Email", "admin@dernek.com");
                                cmd.Parameters.AddWithValue("@Role", (int)UserRole.Yonetici);
                                cmd.Parameters.AddWithValue("@IsActive", true);
                                cmd.Parameters.AddWithValue("@KayitTarihi", DateTime.Now);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Default admin oluşturulurken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Kullanıcı giriş kontrolü
        /// </summary>
        public User Login(string kullaniciAdi, string sifre)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT * FROM Users 
                        WHERE KullaniciAdi = ? AND Sifre = ? AND IsActive = True";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
                        command.Parameters.AddWithValue("@Sifre", sifre);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new User
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    KullaniciAdi = reader["KullaniciAdi"].ToString(),
                                    Sifre = reader["Sifre"].ToString(),
                                    Ad = reader["Ad"].ToString(),
                                    Soyad = reader["Soyad"].ToString(),
                                    Email = reader["Email"]?.ToString(),
                                    Role = (UserRole)Convert.ToInt32(reader["Role"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    KayitTarihi = Convert.ToDateTime(reader["KayitTarihi"]),
                                    UyeID = reader["UyeID"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["UyeID"])
                                };

                                // Son giriş tarihini güncelle
                                UpdateLastLogin(user.UserID);

                                return user;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş kontrolü sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        /// <summary>
        /// Son giriş tarihini günceller
        /// </summary>
        private void UpdateLastLogin(int userID)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE Users SET SonGirisTarihi = ? WHERE UserID = ?";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SonGirisTarihi", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Son giriş güncelleme hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Yeni kullanıcı oluştur
        /// </summary>
        public bool CreateUser(User user)
        {
            try
            {
                if (!user.IsValid())
                {
                    MessageBox.Show(user.GetValidationError(), "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kullanıcı adı tekrarı kontrolü
                if (IsUsernameExists(user.KullaniciAdi))
                {
                    MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Users (KullaniciAdi, Sifre, Ad, Soyad, Email, Role, IsActive, KayitTarihi, UyeID)
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciAdi", user.KullaniciAdi);
                        command.Parameters.AddWithValue("@Sifre", user.Sifre);
                        command.Parameters.AddWithValue("@Ad", user.Ad);
                        command.Parameters.AddWithValue("@Soyad", user.Soyad);
                        command.Parameters.AddWithValue("@Email", user.Email ?? "");
                        command.Parameters.AddWithValue("@Role", (int)user.Role);
                        command.Parameters.AddWithValue("@IsActive", user.IsActive);
                        command.Parameters.AddWithValue("@KayitTarihi", user.KayitTarihi);
                        command.Parameters.AddWithValue("@UyeID", user.UyeID ?? (object)DBNull.Value);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Kullanıcı başarıyla oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı oluşturulurken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        /// <summary>
        /// Kullanıcı adı var mı kontrol et
        /// </summary>
        private bool IsUsernameExists(string kullaniciAdi)
        {
            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE KullaniciAdi = ?";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tüm kullanıcıları getir (sadece yöneticiler için)
        /// </summary>
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Users ORDER BY Ad, Soyad";

                    using (var command = new OleDbCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                KullaniciAdi = reader["KullaniciAdi"].ToString(),
                                Ad = reader["Ad"].ToString(),
                                Soyad = reader["Soyad"].ToString(),
                                Email = reader["Email"]?.ToString(),
                                Role = (UserRole)Convert.ToInt32(reader["Role"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                KayitTarihi = Convert.ToDateTime(reader["KayitTarihi"]),
                                SonGirisTarihi = reader["SonGirisTarihi"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["SonGirisTarihi"]),
                                UyeID = reader["UyeID"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["UyeID"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar getirilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return users;
        }
    }
}