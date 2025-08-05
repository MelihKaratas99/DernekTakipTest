using System;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace DernekTakipSistemi
{
    /// <summary>
    /// Geliştirilmiş Access veritabanı bağlantı yardımcı sınıfı
    /// </summary>
    public class DatabaseHelper
    {
        private readonly string connectionString;
        private readonly string dbPath;

        public DatabaseHelper()
        {
            // Uygulama klasöründe dernek.mdb dosyasını ara
            dbPath = Path.Combine(Application.StartupPath, "dernek.mdb");

            // Bağlantı stringi (farklı provider seçenekleri)
            connectionString = GetConnectionString();

            // Eğer dosya yoksa oluşturmayı dene
            if (!File.Exists(dbPath))
            {
                CreateDatabase();
            }
            else
            {
                // Dosya varsa tablo yapısını kontrol et
                CheckAndCreateTables();
            }
        }

        /// <summary>
        /// Uygun connection string'i döndürür
        /// </summary>
        private string GetConnectionString()
        {
            // Önce Jet 4.0 dene, sonra ACE deneyecek
            return $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={dbPath};";
        }

        /// <summary>
        /// Alternatif connection string (ACE Engine için)
        /// </summary>
        private string GetACEConnectionString()
        {
            return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";
        }

        /// <summary>
        /// Veritabanı bağlantısı döndürür
        /// </summary>
        public OleDbConnection GetConnection()
        {
            try
            {
                var connection = new OleDbConnection(connectionString);
                connection.Open();
                connection.Close();
                return new OleDbConnection(connectionString);
            }
            catch
            {
                // Jet çalışmazsa ACE dene
                try
                {
                    var aceConnectionString = GetACEConnectionString();
                    var connection = new OleDbConnection(aceConnectionString);
                    connection.Open();
                    connection.Close();
                    return new OleDbConnection(aceConnectionString);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Veritabanı bağlantısı kurulamadı. Lütfen Microsoft Access Database Engine'in yüklü olduğundan emin olun.\nHata: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Veritabanı dosyasını oluşturur
        /// </summary>
        private void CreateDatabase()
        {
            try
            {
                // Önce ADOX ile deneme
                if (CreateWithADOX())
                {
                    MessageBox.Show($"Veritabanı başarıyla oluşturuldu: {dbPath}",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CreateTables();
                    return;
                }

                // ADOX başarısızsa manuel template kopyalama
                if (CreateFromTemplate())
                {
                    MessageBox.Show($"Veritabanı template'ten oluşturuldu: {dbPath}",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CreateTables();
                    return;
                }

                // Hiçbiri çalışmazsa kullanıcıdan manuel oluşturmasını iste
                RequestManualCreation();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanı oluşturulurken hata: {ex.Message}\n\n" +
                    "Çözüm önerileri:\n" +
                    "1. Microsoft Access Database Engine 2016 Redistributable indirin\n" +
                    "2. Manuel olarak boş bir Access dosyası oluşturun\n" +
                    "3. Programı yönetici olarak çalıştırın",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ADOX ile veritabanı oluşturma
        /// </summary>
        private bool CreateWithADOX()
        {
            try
            {
                var catalog = Activator.CreateInstance(Type.GetTypeFromProgID("ADOX.Catalog"));
                catalog.GetType().InvokeMember("Create",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null, catalog, new object[] { connectionString });

                System.Runtime.InteropServices.Marshal.ReleaseComObject(catalog);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Template'ten veritabanı oluşturma
        /// </summary>
        private bool CreateFromTemplate()
        {
            try
            {
                // Basit boş Access dosyası template'i (2KB)
                byte[] emptyMdbTemplate = {
                    0x00, 0x01, 0x00, 0x00, 0x53, 0x74, 0x61, 0x6E, 0x64, 0x61, 0x72, 0x64, 0x20, 0x4A, 0x65, 0x74,
                    0x20, 0x44, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    // ... template devamı (basitleştirilmiş)
                };

                // Daha güvenilir yöntem: Kaynaklardan template kopyalama
                string templatePath = Path.Combine(Application.StartupPath, "template.mdb");

                if (File.Exists(templatePath))
                {
                    File.Copy(templatePath, dbPath);
                    return true;
                }

                // Template yoksa minimal dosya oluştur
                File.WriteAllBytes(dbPath, emptyMdbTemplate);

                // Test et
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Manuel oluşturma talebi
        /// </summary>
        private void RequestManualCreation()
        {
            var result = MessageBox.Show(
                "Otomatik veritabanı oluşturulamadı.\n\n" +
                "Manuel olarak oluşturmak ister misiniz?\n" +
                "- Evet: Size yol gösterelim\n" +
                "- Hayır: Program kapatılacak",
                "Veritabanı Oluşturma",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ShowManualInstructions();
            }
            else
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Manuel oluşturma talimatları
        /// </summary>
        private void ShowManualInstructions()
        {
            string instructions = $"Manuel Veritabanı Oluşturma:\n\n" +
                $"1. Microsoft Access'i açın\n" +
                $"2. 'Boş veritabanı' seçin\n" +
                $"3. Dosya adını 'dernek' yapın\n" +
                $"4. Konumu şuraya ayarlayın:\n   {Application.StartupPath}\n" +
                $"5. 'Oluştur' butonuna tıklayın\n" +
                $"6. Access'i kapatın ve programı yeniden başlatın\n\n" +
                $"Alternatif: Boş bir .mdb dosyasını bu klasöre kopyalayın.";

            MessageBox.Show(instructions, "Manuel Oluşturma Talimatları",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Tablo varlığını kontrol eder ve gerekirse oluşturur
        /// </summary>
        private void CheckAndCreateTables()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    // Users tablosu var mı?
                    if (!TableExists(connection, "Users"))
                    {
                        CreateUsersTable(connection);
                    }

                    // Uyeler tablosu var mı?
                    if (!TableExists(connection, "Uyeler"))
                    {
                        CreateUyelerTable(connection);
                    }

                    // Admin kullanıcısı var mı?
                    CreateDefaultAdmin(connection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablo kontrolü sırasında hata: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Tablonun varlığını kontrol eder
        /// </summary>
        private bool TableExists(OleDbConnection connection, string tableName)
        {
            try
            {
                var tables = connection.GetSchema("Tables");
                foreach (System.Data.DataRow row in tables.Rows)
                {
                    if (row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            catch
            {
                // Alternatif kontrol yöntemi
                try
                {
                    using (var command = new OleDbCommand($"SELECT COUNT(*) FROM {tableName}", connection))
                    {
                        command.ExecuteScalar();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gerekli tabloları oluşturur
        /// </summary>
        private void CreateTables()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    // Users tablosu
                    CreateUsersTable(connection);

                    // Uyeler tablosu  
                    CreateUyelerTable(connection);

                    // Varsayılan admin kullanıcısı
                    CreateDefaultAdmin(connection);

                    MessageBox.Show("Tablo yapısı başarıyla oluşturuldu.",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablolar oluşturulurken hata: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Users tablosunu oluşturur
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
                    KayitTarihi DATETIME,
                    SonGirisTarihi DATETIME,
                    UyeID INTEGER
                )";

            try
            {
                using (var command = new OleDbCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Hata logla ama programı durdurma
                System.Diagnostics.Debug.WriteLine($"Users tablosu oluşturma hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Uyeler tablosunu oluşturur
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
                    UyelikTarihi DATETIME,
                    UyelikDurumu TEXT(10) DEFAULT 'Aktif',
                    AidatBorcu CURRENCY DEFAULT 0,
                    SonOdemeTarihi DATETIME
                )";

            try
            {
                using (var command = new OleDbCommand(createUyelerTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Uyeler tablosu oluşturma hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Varsayılan admin kullanıcısı oluşturur
        /// </summary>
        private void CreateDefaultAdmin(OleDbConnection connection)
        {
            try
            {
                // Admin var mı kontrol et
                string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Role = 2";
                using (var command = new OleDbCommand(checkAdmin, connection))
                {
                    var result = command.ExecuteScalar();
                    int adminCount = result != null ? Convert.ToInt32(result) : 0;

                    if (adminCount == 0)
                    {
                        // Varsayılan admin oluştur
                        string insertAdmin = @"
                            INSERT INTO Users (KullaniciAdi, Sifre, Ad, Soyad, Email, Role, IsActive, KayitTarihi)
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                        using (var cmd = new OleDbCommand(insertAdmin, connection))
                        {
                            cmd.Parameters.AddWithValue("@KullaniciAdi", "admin");
                            cmd.Parameters.AddWithValue("@Sifre", "123456");
                            cmd.Parameters.AddWithValue("@Ad", "Sistem");
                            cmd.Parameters.AddWithValue("@Soyad", "Yöneticisi");
                            cmd.Parameters.AddWithValue("@Email", "admin@dernek.com");
                            cmd.Parameters.AddWithValue("@Role", 2); // Yönetici
                            cmd.Parameters.AddWithValue("@IsActive", true);
                            cmd.Parameters.AddWithValue("@KayitTarihi", DateTime.Now);

                            cmd.ExecuteNonQuery();
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
            catch
            {
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