using System;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace DernekTakipSistemi
{
    /// <summary>
    /// Access 2001 XP uyumlu veritabanı yardımcı sınıfı
    /// </summary>
    public class DatabaseHelper
    {
        private readonly string connectionString;
        private readonly string dbPath;

        public DatabaseHelper()
        {
            // Uygulama klasöründe dernek.mdb dosyasını ara
            dbPath = Path.Combine(Application.StartupPath, "dernek.mdb");

            // Access 2001 XP için Jet 4.0 kullan
            connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={dbPath};";

            // Eğer dosya yoksa oluşturmasını iste
            if (!File.Exists(dbPath))
            {
                CreateDatabaseManually();
            }
            else
            {
                // Bağlantı testi yap
                if (!TestConnectionOnStartup())
                {
                    MessageBox.Show($"Veritabanı bağlantısı kurulamadı!\n\nDosya: {dbPath}\n\nLütfen dosyanın var olduğundan ve erişilebilir olduğundan emin olun.",
                        "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Varolan dosyada tabloları kontrol et ve oluştur
                EnsureTablesExist();
            }
        }

        /// <summary>
        /// Başlangıçta bağlantı testi yapar
        /// </summary>
        private bool TestConnectionOnStartup()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    // Basit bir sorgu çalıştır
                    using (var command = new OleDbCommand("SELECT 1", connection))
                    {
                        command.ExecuteScalar();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bağlantı testi hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Veritabanı bağlantısı döndürür
        /// </summary>
        public OleDbConnection GetConnection()
        {
            return new OleDbConnection(connectionString);
        }

        /// <summary>
        /// Manuel veritabanı oluşturma talimatı
        /// </summary>
        private void CreateDatabaseManually()
        {
            DialogResult result = MessageBox.Show(
                $"Veritabanı dosyası bulunamadı.\n\n" +
                $"Konum: {dbPath}\n\n" +
                $"Lütfen aşağıdaki adımları takip edin:\n\n" +
                $"1. Microsoft Access 2001'i açın\n" +
                $"2. 'Boş Veritabanı' oluşturun\n" +
                $"3. 'dernek.mdb' adıyla kaydedin\n" +
                $"4. Dosyayı şu konuma kaydedin:\n" +
                $"   {Application.StartupPath}\n\n" +
                $"Sonra 'Tamam'a basın.",
                "Veritabanı Oluşturun",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (result == DialogResult.OK)
            {
                // Kullanıcı dosyayı oluşturduğunu söylüyor, kontrol et
                if (File.Exists(dbPath))
                {
                    MessageBox.Show("Veritabanı dosyası bulundu! Tablolar oluşturuluyor...",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EnsureTablesExist();
                }
                else
                {
                    MessageBox.Show("Veritabanı dosyası hâlâ bulunamadı. Program kapatılıyor.",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Tabloların var olduğundan emin olur
        /// </summary>
        private void EnsureTablesExist()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    // Users tablosunu kontrol et ve oluştur
                    if (!TableExists(connection, "Users"))
                    {
                        CreateUsersTable(connection);
                        CreateDefaultAdmin(connection);
                        MessageBox.Show("Users tablosu oluşturuldu!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Uyeler tablosunu kontrol et ve oluştur
                    if (!TableExists(connection, "Uyeler"))
                    {
                        CreateUyelerTable(connection);
                        MessageBox.Show("Uyeler tablosu oluşturuldu!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Admin yoksa oluştur
                    EnsureDefaultAdminExists(connection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablolar kontrol edilirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tablonun var olup olmadığını kontrol eder
        /// </summary>
        private bool TableExists(OleDbConnection connection, string tableName)
        {
            try
            {
                var tables = connection.GetSchema("Tables");
                foreach (System.Data.DataRow row in tables.Rows)
                {
                    if (row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Users tablosunu oluşturur (Access 2001 uyumlu)
        /// </summary>
        private void CreateUsersTable(OleDbConnection connection)
        {
            string createUsersTable = @"
                CREATE TABLE Users (
                    UserID COUNTER PRIMARY KEY,
                    KullaniciAdi TEXT(50) NOT NULL,
                    Sifre TEXT(255) NOT NULL,
                    Ad TEXT(50) NOT NULL,
                    Soyad TEXT(50) NOT NULL,
                    Email TEXT(100),
                    Role INTEGER DEFAULT 1,
                    IsActive YESNO DEFAULT True,
                    UyelikTarihi DATETIME DEFAULT Now(),
                    SonGirisTarihi DATETIME,
                    UyeID INTEGER
                )";

            using (var command = new OleDbCommand(createUsersTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Uyeler tablosunu oluşturur (Access 2001 uyumlu)
        /// </summary>
        private void CreateUyelerTable(OleDbConnection connection)
        {
            string createUyelerTable = @"
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

            using (var command = new OleDbCommand(createUyelerTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Varsayılan admin kullanıcısı oluşturur
        /// </summary>
        private void CreateDefaultAdmin(OleDbConnection connection)
        {
            string insertAdmin = @"
                INSERT INTO Users (KullaniciAdi, Sifre, Ad, Soyad, Email, Role, IsActive, UyelikTarihi)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

            using (var cmd = new OleDbCommand(insertAdmin, connection))
            {
                cmd.Parameters.AddWithValue("@KullaniciAdi", "admin");
                cmd.Parameters.AddWithValue("@Sifre", "123456");
                cmd.Parameters.AddWithValue("@Ad", "Sistem");
                cmd.Parameters.AddWithValue("@Soyad", "Yöneticisi");
                cmd.Parameters.AddWithValue("@Email", "admin@dernek.com");
                cmd.Parameters.AddWithValue("@Role", 2);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@UyelikTarihi", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Admin kullanıcısının var olduğundan emin olur
        /// </summary>
        private void EnsureDefaultAdminExists(OleDbConnection connection)
        {
            try
            {
                string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Role = 2";
                using (var command = new OleDbCommand(checkAdmin, connection))
                {
                    object result = command.ExecuteScalar();
                    int adminCount = result != null ? Convert.ToInt32(result) : 0;

                    if (adminCount == 0)
                    {
                        CreateDefaultAdmin(connection);
                        MessageBox.Show("Varsayılan admin kullanıcısı oluşturuldu!\nKullanıcı: admin\nŞifre: 123456",
                            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Admin kontrol hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Veritabanı bağlantısını test eder
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bağlantı testi hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// SQL komutunu çalıştırır ve etkilenen satır sayısını döndürür
        /// </summary>
        public int ExecuteNonQuery(string sql, params OleDbParameter[] parameters)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new OleDbCommand(sql, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Tek değer döndüren sorguları çalıştırır
        /// </summary>
        public object ExecuteScalar(string sql, params OleDbParameter[] parameters)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new OleDbCommand(sql, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL hatası: {ex.Message}");
            }
        }
    }
}