using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;

namespace DernekTakipSistemi
{
    public partial class UyeEkleDuzenleForm : Form
    {
        // Renkler
        private readonly Color PrimaryColor = Color.FromArgb(44, 62, 80);
        private readonly Color AccentColor = Color.FromArgb(52, 152, 219);
        private readonly Color SuccessColor = Color.FromArgb(39, 174, 96);
        private readonly Color DangerColor = Color.FromArgb(231, 76, 60);
        private readonly Color LightGray = Color.FromArgb(236, 240, 241);

        // Form elemanları
        private TextBox txtTC, txtAd, txtSoyad, txtTelefon, txtEmail, txtAdres;
        private ComboBox cmbDurum;
        private DateTimePicker dtpUyelikTarihi;
        private Button btnKaydet, btnIptal;
        private Label lblBaslik;

        // Servis ve model
        private UyeService uyeService;
        private Uye editingUye;
        private bool isEditMode;

        public UyeEkleDuzenleForm(Uye uye = null)
        {
            uyeService = new UyeService();
            editingUye = uye;
            isEditMode = uye != null;

            InitializeComponent();
            InitializeForm();
            SetupControls();

            if (isEditMode)
            {
                LoadUyeData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private void InitializeForm()
        {
            this.Size = new Size(500, 550);
            this.Text = isEditMode ? "Üye Düzenle" : "Yeni Üye Ekle";
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
                Size = new Size(450, 500),
                Location = new Point(25, 25),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Başlık
            lblBaslik = new Label
            {
                Text = isEditMode ? "ÜYE BİLGİLERİNİ DÜZENLE" : "YENİ ÜYE EKLE",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 20),
                Size = new Size(410, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // TC Kimlik No
            Label lblTC = new Label
            {
                Text = "TC Kimlik No:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 70),
                AutoSize = true
            };

            txtTC = new TextBox
            {
                Location = new Point(30, 95),
                Size = new Size(390, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 11
            };

            // Ad
            Label lblAd = new Label
            {
                Text = "Ad:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 130),
                AutoSize = true
            };

            txtAd = new TextBox
            {
                Location = new Point(30, 155),
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
                Location = new Point(240, 130),
                AutoSize = true
            };

            txtSoyad = new TextBox
            {
                Location = new Point(240, 155),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Telefon
            Label lblTelefon = new Label
            {
                Text = "Telefon:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 190),
                AutoSize = true
            };

            txtTelefon = new TextBox
            {
                Location = new Point(30, 215),
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
                Location = new Point(240, 190),
                AutoSize = true
            };

            txtEmail = new TextBox
            {
                Location = new Point(240, 215),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Üyelik Tarihi
            Label lblUyelikTarihi = new Label
            {
                Text = "Üyelik Tarihi:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 250),
                AutoSize = true
            };

            dtpUyelikTarihi = new DateTimePicker
            {
                Location = new Point(30, 275),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            // Durum
            Label lblDurum = new Label
            {
                Text = "Durum:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(240, 250),
                AutoSize = true
            };

            cmbDurum = new ComboBox
            {
                Location = new Point(240, 275),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDurum.Items.AddRange(new string[] { "Aktif", "Pasif" });
            cmbDurum.SelectedIndex = 0;

            // Adres
            Label lblAdres = new Label
            {
                Text = "Adres:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 310),
                AutoSize = true
            };

            txtAdres = new TextBox
            {
                Location = new Point(30, 335),
                Size = new Size(390, 60),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Butonlar
            btnKaydet = new Button
            {
                Text = isEditMode ? "GÜNCELLE" : "KAYDET",
                Location = new Point(160, 420),
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
                Location = new Point(300, 420),
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
            mainPanel.Controls.AddRange(new Control[] {
                lblBaslik, lblTC, txtTC, lblAd, txtAd, lblSoyad, txtSoyad,
                lblTelefon, txtTelefon, lblEmail, txtEmail, lblUyelikTarihi, dtpUyelikTarihi,
                lblDurum, cmbDurum, lblAdres, txtAdres, btnKaydet, btnIptal
            });

            this.Controls.Add(mainPanel);
        }

        private void LoadUyeData()
        {
            if (editingUye != null)
            {
                txtTC.Text = editingUye.TC;
                txtAd.Text = editingUye.Ad;
                txtSoyad.Text = editingUye.Soyad;
                txtTelefon.Text = editingUye.Telefon;
                txtEmail.Text = editingUye.Email;
                txtAdres.Text = editingUye.Adres;
                dtpUyelikTarihi.Value = editingUye.UyelikTarihi;
                cmbDurum.SelectedItem = editingUye.UyelikDurumu;
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtTC.Text))
                {
                    MessageBox.Show("TC Kimlik No boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTC.Focus();
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

                // TC tekrar kontrolü (edit modunda mevcut üyeyi hariç tut)
                if (uyeService.TCVarMi(txtTC.Text.Trim(), isEditMode ? editingUye.UyeID : 0))
                {
                    MessageBox.Show("Bu TC Kimlik No ile kayıtlı bir üye zaten var.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTC.Focus();
                    return;
                }

                // Üye nesnesi oluştur
                Uye uye = isEditMode ? editingUye : new Uye();

                uye.TC = txtTC.Text.Trim();
                uye.Ad = txtAd.Text.Trim();
                uye.Soyad = txtSoyad.Text.Trim();
                uye.Telefon = txtTelefon.Text.Trim();
                uye.Email = txtEmail.Text.Trim();
                uye.Adres = txtAdres.Text.Trim();
                uye.UyelikTarihi = dtpUyelikTarihi.Value;
                uye.UyelikDurumu = cmbDurum.SelectedItem.ToString();

                // AidatBorcu'yu sadece edit modunda koru, yeni eklemede 0 yap
                if (!isEditMode)
                {
                    uye.AidatBorcu = 0; // Yeni üye - borç yok
                }
                // Edit modunda AidatBorcu değeri korunur (zaten editingUye'den geldi)

                // Kaydet
                bool success = isEditMode ? uyeService.UyeGuncelle(uye) : uyeService.UyeEkle(uye);

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