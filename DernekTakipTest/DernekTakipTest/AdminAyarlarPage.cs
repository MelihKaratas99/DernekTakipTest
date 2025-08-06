using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages.Admin
{
    public class AdminAyarlarPage : BaseMenuPage
    {
        public override string PageTitle => "Ayarlar";
        public override string PageIcon => "⚙️";

        protected override void InitializePage()
        {
            CreateSettingsSection();
        }

        private void CreateSettingsSection()
        {
            Panel settingsPanel = CreateContentPanel(new Point(0, 0), new Size(970, 500));

            Label settingsTitle = new Label
            {
                Text = "🔧 Sistem Ayarları",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Ayar kategorileri
            CreateUserManagementSection(settingsPanel);
            CreateSystemSettingsSection(settingsPanel);
            CreateDatabaseSection(settingsPanel);

            settingsPanel.Controls.Add(settingsTitle);
            MainContentPanel.Controls.Add(settingsPanel);
        }

        private void CreateUserManagementSection(Panel parent)
        {
            Label userTitle = new Label
            {
                Text = "👤 Kullanıcı Yönetimi",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = AccentColor,
                Location = new Point(50, 70),
                AutoSize = true
            };

            Button kullaniciEkleBtn = CreateActionButton("+ Kullanıcı Ekle", new Point(50, 100), SuccessColor);
            Button kullaniciListeleBtn = CreateActionButton("📋 Kullanıcı Listesi", new Point(220, 100), AccentColor);
            Button sifreResetBtn = CreateActionButton("🔑 Şifre Sıfırla", new Point(390, 100), WarningColor);

            // Event handlers - Gerçek işlevler
            kullaniciEkleBtn.Click += KullaniciEkleBtn_Click;
            kullaniciListeleBtn.Click += KullaniciListeleBtn_Click;
            sifreResetBtn.Click += (s, e) => MessageBox.Show("Şifre sıfırlama formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            parent.Controls.AddRange(new Control[] {
        userTitle, kullaniciEkleBtn, kullaniciListeleBtn, sifreResetBtn
    });
        }

        private void CreateSystemSettingsSection(Panel parent)
        {
            Label systemTitle = new Label
            {
                Text = "⚙️ Sistem Ayarları",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = AccentColor,
                Location = new Point(50, 170),
                AutoSize = true
            };

            Button aidatAyarlariBtn = CreateActionButton("💰 Aidat Ayarları", new Point(50, 200), SuccessColor);
            Button emailAyarlariBtn = CreateActionButton("📧 E-posta Ayarları", new Point(220, 200), AccentColor);
            Button yedeklemeBtn = CreateActionButton("💾 Yedekleme", new Point(390, 200), DarkGray);

            // Event handlers
            aidatAyarlariBtn.Click += (s, e) => MessageBox.Show("Aidat ayarları formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            emailAyarlariBtn.Click += (s, e) => MessageBox.Show("E-posta ayarları formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            yedeklemeBtn.Click += (s, e) => MessageBox.Show("Yedekleme işlemi başlatılıyor...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            parent.Controls.AddRange(new Control[] {
                systemTitle, aidatAyarlariBtn, emailAyarlariBtn, yedeklemeBtn
            });
        }

        private void CreateDatabaseSection(Panel parent)
        {
            Label dbTitle = new Label
            {
                Text = "🗄️ Veritabanı İşlemleri",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = AccentColor,
                Location = new Point(50, 270),
                AutoSize = true
            };

            Button dbTestBtn = CreateActionButton("🔍 Bağlantı Testi", new Point(50, 300), AccentColor);
            Button dbYedekBtn = CreateActionButton("💾 Yedek Al", new Point(220, 300), SuccessColor);
            Button dbTemizleBtn = CreateActionButton("🧹 Temizle", new Point(390, 300), DangerColor);

            // Event handlers
            dbTestBtn.Click += DbTestBtn_Click;
            dbYedekBtn.Click += (s, e) => MessageBox.Show("Veritabanı yedeği alınıyor...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dbTemizleBtn.Click += DbTemizleBtn_Click;

            parent.Controls.AddRange(new Control[] {
                dbTitle, dbTestBtn, dbYedekBtn, dbTemizleBtn
            });
        }

        private void KullaniciEkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                KullaniciEkleDuzenleForm form = new KullaniciEkleDuzenleForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Kullanıcı başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı ekleme formu açılırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void KullaniciListeleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                UserService userService = new UserService();
                List<User> users = userService.GetAllUsers();

                // Basit liste formu
                Form listForm = new Form
                {
                    Text = "Kullanıcı Listesi",
                    Size = new Size(800, 600),
                    StartPosition = FormStartPosition.CenterParent
                };

                DataGridView userGrid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    BackgroundColor = Color.White
                };

                userGrid.Columns.Add("UserID", "ID");
                userGrid.Columns.Add("KullaniciAdi", "Kullanıcı Adı");
                userGrid.Columns.Add("AdSoyad", "Ad Soyad");
                userGrid.Columns.Add("Email", "E-posta");
                userGrid.Columns.Add("Role", "Rol");
                userGrid.Columns.Add("IsActive", "Durum");
                userGrid.Columns.Add("KayitTarihi", "Kayıt Tarihi");

                userGrid.Columns[0].Visible = false;

                foreach (User user in users)
                {
                    userGrid.Rows.Add(
                        user.UserID,
                        user.KullaniciAdi,
                        user.AdSoyad,
                        user.Email,
                        user.RoleText,
                        user.IsActive ? "Aktif" : "Pasif",
                        user.KayitTarihi.ToString("dd.MM.yyyy")
                    );
                }

                listForm.Controls.Add(userGrid);
                listForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı listesi alınırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DbTestBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                bool isConnected = dbHelper.TestConnection();

                if (isConnected)
                {
                    MessageBox.Show("Veritabanı bağlantısı başarılı!", "Bağlantı Testi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Veritabanı bağlantısı başarısız!", "Bağlantı Testi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bağlantı testi sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DbTemizleBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bu işlem tüm verileri silecek! Devam etmek istediğinizden emin misiniz?",
                "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Veritabanı temizleme işlemi burada yapılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public override void LoadPage()
        {
            // Ayarlar sayfası yüklendiğinde
        }

        public override void RefreshPage()
        {
            // Ayarlar sayfası yenilendiğinde
        }
    }
}