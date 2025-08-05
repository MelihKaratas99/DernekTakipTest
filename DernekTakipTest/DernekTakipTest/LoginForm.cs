using System;
using System.Drawing;
using System.Windows.Forms;
using DernekTakipSistemi.Pages.Member;

namespace DernekTakipSistemi
{
    public partial class LoginForm : Form
    {
        // Renkler
        private readonly Color PrimaryColor = Color.FromArgb(44, 62, 80);
        private readonly Color AccentColor = Color.FromArgb(52, 152, 219);
        private readonly Color SuccessColor = Color.FromArgb(39, 174, 96);
        private readonly Color DangerColor = Color.FromArgb(231, 76, 60);
        private readonly Color LightGray = Color.FromArgb(236, 240, 241);

        // Form elemanları
        private TextBox txtKullaniciAdi, txtSifre;
        private Button btnGiris, btnCikis;
        private Label lblBaslik, lblKullaniciAdi, lblSifre, lblBilgi;
        private Panel loginPanel;

        // Servis
        private UserService userService;

        public LoginForm()
        {
            userService = new UserService();
            InitializeForm();
            SetupControls();
        }

        private void InitializeForm()
        {
            this.Size = new Size(450, 350);
            this.Text = "Dernek Takip Sistemi - Giriş";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = LightGray;
            this.Icon = null;
        }

        private void SetupControls()
        {
            // Ana login paneli
            loginPanel = new Panel
            {
                Size = new Size(350, 280),
                Location = new Point(50, 35),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Başlık
            lblBaslik = new Label
            {
                Text = "DERNEK TAKİP SİSTEMİ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(50, 30),
                Size = new Size(250, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Kullanıcı adı label
            lblKullaniciAdi = new Label
            {
                Text = "Kullanıcı Adı:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 80),
                AutoSize = true
            };

            // Kullanıcı adı textbox
            txtKullaniciAdi = new TextBox
            {
                Location = new Point(30, 105),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Şifre label
            lblSifre = new Label
            {
                Text = "Şifre:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 140),
                AutoSize = true
            };

            // Şifre textbox
            txtSifre = new TextBox
            {
                Location = new Point(30, 165),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Giriş butonu
            btnGiris = new Button
            {
                Text = "GİRİŞ YAP",
                Location = new Point(30, 210),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = SuccessColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Click += BtnGiris_Click;

            // Çıkış butonu
            btnCikis = new Button
            {
                Text = "ÇIKIŞ",
                Location = new Point(180, 210),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = DangerColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCikis.FlatAppearance.BorderSize = 0;
            btnCikis.Click += BtnCikis_Click;

            // Bilgi label
            lblBilgi = new Label
            {
                Text = "Varsayılan Giriş: admin / 123456",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(30, 255),
                Size = new Size(290, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Kontrolleri panele ekle
            loginPanel.Controls.AddRange(new Control[] {
                lblBaslik, lblKullaniciAdi, txtKullaniciAdi, lblSifre, txtSifre,
                btnGiris, btnCikis, lblBilgi
            });

            // Paneli forma ekle
            this.Controls.Add(loginPanel);

            // Enter tuşu için event
            txtSifre.KeyPress += TxtSifre_KeyPress;
            txtKullaniciAdi.KeyPress += TxtKullaniciAdi_KeyPress;

            // İlk focus
            txtKullaniciAdi.Focus();
        }

        private void TxtKullaniciAdi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtSifre.Focus();
                e.Handled = true;
            }
        }

        private void TxtSifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnGiris_Click(sender, e);
                e.Handled = true;
            }
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
            try
            {
                string kullaniciAdi = txtKullaniciAdi.Text.Trim();
                string sifre = txtSifre.Text;

                // Validation
                if (string.IsNullOrEmpty(kullaniciAdi))
                {
                    MessageBox.Show("Kullanıcı adını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtKullaniciAdi.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(sifre))
                {
                    MessageBox.Show("Şifrenizi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSifre.Focus();
                    return;
                }

                // Giriş kontrolü
                btnGiris.Enabled = false;
                btnGiris.Text = "Kontrol ediliyor...";

                User user = userService.Login(kullaniciAdi, sifre);

                if (user != null)
                {
                    // Başarılı giriş
                    CurrentUser.Login(user);

                    MessageBox.Show($"Hoş geldiniz, {user.AdSoyad}!\nRol: {user.RoleText}", "Giriş Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Debug bilgileri
                    System.Diagnostics.Debug.WriteLine($"User Role: {user.Role}");
                    System.Diagnostics.Debug.WriteLine($"Is Admin: {user.IsAdmin}");

                    try
                    {
                        // Role göre ana formu aç
                        this.Hide();

                        if (user.IsAdmin)
                        {
                            System.Diagnostics.Debug.WriteLine("Admin formu açılıyor...");

                            // Yönetici paneli
                            AdminMainForm adminForm = new AdminMainForm();
                            adminForm.FormClosed += (s, args) =>
                            {
                                this.Show();
                                CurrentUser.Logout();
                            };
                            adminForm.Show();
                            adminForm.WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Üye formu açılıyor...");

                            // Üye paneli
                            MemberMainForm memberForm = new MemberMainForm();
                            memberForm.FormClosed += (s, args) =>
                            {
                                this.Show();
                                CurrentUser.Logout();
                            };
                            memberForm.Show();
                            memberForm.WindowState = FormWindowState.Maximized;
                        }
                    }
                    catch (Exception formEx)
                    {
                        MessageBox.Show($"Form açılırken hata: {formEx.Message}\n\nDetay: {formEx.StackTrace}",
                            "Form Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Giriş Başarısız",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSifre.Clear();
                    txtKullaniciAdi.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş sırasında hata: {ex.Message}\n\nDetay: {ex.StackTrace}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGiris.Enabled = true;
                btnGiris.Text = "GİRİŞ YAP";
            }
        }

        private void BtnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
            base.OnFormClosing(e);
        }
    }
}