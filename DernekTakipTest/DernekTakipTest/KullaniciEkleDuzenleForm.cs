using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DernekTakipSistemi
{
    public partial class KullaniciEkleDuzenleForm : Form
    {
        // Renkler
        private readonly Color PrimaryColor = Color.FromArgb(44, 62, 80);
        private readonly Color AccentColor = Color.FromArgb(52, 152, 219);
        private readonly Color SuccessColor = Color.FromArgb(39, 174, 96);
        private readonly Color DangerColor = Color.FromArgb(231, 76, 60);
        private readonly Color LightGray = Color.FromArgb(236, 240, 241);

        // Form elemanları
        private TextBox txtKullaniciAdi, txtSifre, txtSifreTekrar, txtAd, txtSoyad, txtEmail;
        private ComboBox cmbRole, cmbUye;
        private CheckBox chkAktif;
        private Button btnKaydet, btnIptal;
        private Label lblBaslik, lblSifreInfo;

        // Servisler
        private UserService userService;
        private UyeService uyeService;
        private User editingUser;
        private bool isEditMode;

        public KullaniciEkleDuzenleForm(User user = null)
        {
            userService = new UserService();
            uyeService = new UyeService();
            editingUser = user;
            isEditMode = user != null;

            InitializeComponent();
            InitializeForm();
            SetupControls();
            LoadUyeler();

            if (isEditMode)
            {
                LoadUserData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private void InitializeForm()
        {
            this.Size = new Size(500, 650);
            this.Text = isEditMode ? "Kullanıcı Düzenle" : "Yeni Kullanıcı Ekle";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = LightGray;
        }

        private void SetupControls()
        {
            // Ana panel
            Panel mainPanel = new Panel
            {
                Size = new Size(450, 600),
                Location = new Point(25, 25),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Başlık
            lblBaslik = new Label
            {
                Text = isEditMode ? "KULLANICI BİLGİLERİNİ DÜZENLE" : "YENİ KULLANICI EKLE",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 20),
                Size = new Size(410, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Kullanıcı Adı
            Label lblKullaniciAdi = new Label
            {
                Text = "Kullanıcı Adı:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 70),
                AutoSize = true
            };

            txtKullaniciAdi = new TextBox
            {
                Location = new Point(30, 95),
                Size = new Size(390, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Şifre
            Label lblSifre = new Label
            {
                Text = "Şifre:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 130),
                AutoSize = true
            };

            txtSifre = new TextBox
            {
                Location = new Point(30, 155),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Şifre Tekrar
            Label lblSifreTekrar = new Label
            {
                Text = "Şifre Tekrar:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(240, 130),
                AutoSize = true
            };

            txtSifreTekrar = new TextBox
            {
                Location = new Point(240, 155),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Şifre bilgi etiketi (edit modu için)
            lblSifreInfo = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(30, 185),
                Size = new Size(390, 15),
                Visible = false
            };

            // Ad
            Label lblAd = new Label
            {
                Text = "Ad:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 210),
                AutoSize = true
            };

            txtAd = new TextBox
            {
                Location = new Point(30, 235),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Soyad
            Label lblSoyad = new Label
            {
                Text = "Soyad:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(240, 210),
                AutoSize = true
            };

            txtSoyad = new TextBox
            {
                Location = new Point(240, 235),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Email
            Label lblEmail = new Label
            {
                Text = "E-posta:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 270),
                AutoSize = true
            };

            txtEmail = new TextBox
            {
                Location = new Point(30, 295),
                Size = new Size(390, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Rol
            Label lblRole = new Label
            {
                Text = "Kullanıcı Rolü:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 330),
                AutoSize = true
            };

            cmbRole = new ComboBox
            {
                Location = new Point(30, 355),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.Add("Üye");
            cmbRole.Items.Add("Yönetici");
            cmbRole.SelectedIndex = 0;
            cmbRole.SelectedIndexChanged += CmbRole_SelectedIndexChanged;

            // Üye Seçimi (sadece Üye rolü için)
            Label lblUye = new Label
            {
                Text = "Bağlı Üye:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(240, 330),
                AutoSize = true
            };

            cmbUye = new ComboBox
            {
                Location = new Point(240, 355),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Aktif/Pasif
            chkAktif = new CheckBox
            {
                Text = "Kullanıcı Aktif",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 400),
                Checked = true,
                AutoSize = true
            };

            // Hızlı test kullanıcısı butonu (sadece yeni ekleme modunda)
            Button btnTestUye = null;
            if (!isEditMode)
            {
                btnTestUye = new Button
                {
                    Text = "Test Üyesi Oluştur",
                    Location = new Point(240, 395),
                    Size = new Size(120, 25),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AccentColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnTestUye.FlatAppearance.BorderSize = 0;
                btnTestUye.Click += BtnTestUye_Click;
            }

            // Butonlar
            btnKaydet = new Button
            {
                Text = isEditMode ? "GÜNCELLE" : "KAYDET",
                Location = new Point(160, 450),
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = SuccessColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnKaydet.FlatAppearance.BorderSize = 0;
            btnKaydet.Click += BtnKaydet_Click;

            btnIptal = new Button
            {
                Text = "İPTAL",
                Location = new Point(300, 450),
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = DangerColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnIptal.FlatAppearance.BorderSize = 0;
            btnIptal.Click += BtnIptal_Click;

            // Kontrolleri panele ekle
            List<Control> controls = new List<Control> {
                lblBaslik, lblKullaniciAdi, txtKullaniciAdi, lblSifre, txtSifre, lblSifreTekrar, txtSifreTekrar, lblSifreInfo,
                lblAd, txtAd, lblSoyad, txtSoyad, lblEmail, txtEmail, lblRole, cmbRole, lblUye, cmbUye,
                chkAktif, btnKaydet, btnIptal
            };

            if (btnTestUye != null)
                controls.Add(btnTestUye);

            mainPanel.Controls.AddRange(controls.ToArray());
            this.Controls.Add(mainPanel);
        }

        private void BtnTestUye_Click(object sender, EventArgs e)
        {
            // Hızlı test verisi doldur
            txtKullaniciAdi.Text = "testuye";
            txtSifre.Text = "123456";
            txtSifreTekrar.Text = "123456";
            txtAd.Text = "Test";
            txtSoyad.Text = "Üye";
            txtEmail.Text = "testuye@dernek.com";
            cmbRole.SelectedIndex = 0; // Üye

            MessageBox.Show("Test üye bilgileri dolduruldu!\nKullanıcı Adı: testuye\nŞifre: 123456",
                "Test Verisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadUyeler()
        {
            try
            {
                cmbUye.Items.Clear();
                cmbUye.Items.Add("-- Seçiniz --");

                List<Uye> uyeler = uyeService.TumUyeleriGetir();
                foreach (Uye uye in uyeler)
                {
                    cmbUye.Items.Add($"{uye.AdSoyad} ({uye.TC})");
                    cmbUye.Tag = cmbUye.Tag ?? new List<Uye>();
                    ((List<Uye>)cmbUye.Tag).Add(uye);
                }

                cmbUye.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Üyeler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserData()
        {
            if (editingUser != null)
            {
                txtKullaniciAdi.Text = editingUser.KullaniciAdi;
                txtAd.Text = editingUser.Ad;
                txtSoyad.Text = editingUser.Soyad;
                txtEmail.Text = editingUser.Email;
                cmbRole.SelectedIndex = editingUser.Role == UserRole.Yonetici ? 1 : 0;
                chkAktif.Checked = editingUser.IsActive;

                // Üye bağlantısı varsa seç
                if (editingUser.UyeID.HasValue)
                {
                    List<Uye> uyeler = (List<Uye>)cmbUye.Tag;
                    if (uyeler != null)
                    {
                        for (int i = 0; i < uyeler.Count; i++)
                        {
                            if (uyeler[i].UyeID == editingUser.UyeID.Value)
                            {
                                cmbUye.SelectedIndex = i + 1; // +1 çünkü ilk item "-- Seçiniz --"
                                break;
                            }
                        }
                    }
                }

                // Edit modunda şifre bilgisi
                lblSifreInfo.Text = "Şifre değiştirmek istemiyorsanız boş bırakın";
                lblSifreInfo.Visible = true;
                txtSifre.UseSystemPasswordChar = false;
                txtSifreTekrar.UseSystemPasswordChar = false;
            }
        }

        private void CmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Üye rolü seçilirse üye seçimi zorunlu, yönetici rolünde isteğe bağlı
            bool isUyeRole = cmbRole.SelectedIndex == 0;
            cmbUye.Enabled = true; // Her iki durumda da aktif
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text))
                {
                    MessageBox.Show("Kullanıcı adı boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtKullaniciAdi.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAd.Text))
                {
                    MessageBox.Show("Ad boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAd.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSoyad.Text))
                {
                    MessageBox.Show("Soyad boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoyad.Focus();
                    return;
                }

                // Şifre kontrolü
                if (!isEditMode || !string.IsNullOrEmpty(txtSifre.Text))
                {
                    if (string.IsNullOrWhiteSpace(txtSifre.Text))
                    {
                        MessageBox.Show("Şifre boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSifre.Focus();
                        return;
                    }

                    if (txtSifre.Text != txtSifreTekrar.Text)
                    {
                        MessageBox.Show("Şifreler eşleşmiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSifreTekrar.Focus();
                        return;
                    }
                }

                // User nesnesi oluştur
                User user = isEditMode ? editingUser : new User();

                user.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                user.Ad = txtAd.Text.Trim();
                user.Soyad = txtSoyad.Text.Trim();
                user.Email = txtEmail.Text.Trim();
                user.Role = cmbRole.SelectedIndex == 1 ? UserRole.Yonetici : UserRole.Uye;
                user.IsActive = chkAktif.Checked;

                // Şifre güncelleme (edit modunda sadece dolu ise)
                if (!isEditMode || !string.IsNullOrEmpty(txtSifre.Text))
                {
                    user.Sifre = txtSifre.Text;
                }

                // Üye bağlantısı
                if (cmbUye.SelectedIndex > 0)
                {
                    List<Uye> uyeler = (List<Uye>)cmbUye.Tag;
                    if (uyeler != null && cmbUye.SelectedIndex - 1 < uyeler.Count)
                    {
                        user.UyeID = uyeler[cmbUye.SelectedIndex - 1].UyeID;
                    }
                }
                else
                {
                    user.UyeID = null;
                }

                // Kaydet
                bool success;
                if (isEditMode)
                {
                    // UserService'te UpdateUser metodu yoksa CreateUser kullan
                    try
                    {
                        success = userService.UpdateUser(user);
                    }
                    catch
                    {
                        success = userService.CreateUser(user);
                    }
                }
                else
                {
                    success = userService.CreateUser(user);
                }

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnIptal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}