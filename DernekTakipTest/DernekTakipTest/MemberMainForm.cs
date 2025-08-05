using System;
using System.Drawing;
using System.Windows.Forms;
using DernekTakipSistemi.Pages;
using DernekTakipSistemi.Pages.Member;

namespace DernekTakipSistemi
{
    public partial class MemberMainForm : Form
    {
        // Renkler
        private readonly Color PrimaryColor = Color.FromArgb(44, 62, 80);
        private readonly Color AccentColor = Color.FromArgb(52, 152, 219);
        private readonly Color SuccessColor = Color.FromArgb(39, 174, 96);
        private readonly Color DangerColor = Color.FromArgb(231, 76, 60);
        private readonly Color LightGray = Color.FromArgb(236, 240, 241);
        private readonly Color DarkGray = Color.FromArgb(149, 165, 166);

        // UI Elemanları
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Panel headerPanel;
        private Label headerLabel;
        private Button logoutButton;

        // Menü butonları
        private Button btnAnaSayfa;
        private Button btnAidatTakibi;
        private Button btnEtkinlikler;
        private Button btnAyarlar;

        // Aktif sayfa
        private BaseMenuPage currentPage;

        public MemberMainForm()
        {
            InitializeComponent();
            SetupForm();
            CreateHeader();
            CreateSidebar();
            CreateContentArea();
            LoadPage(new MemberAnaSayfaPage());
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private void SetupForm()
        {
            // Form ayarları
            this.Size = new Size(1280, 720);
            this.Text = "Dernek Takip Sistemi - Üye Paneli";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = LightGray;
            this.MinimumSize = new Size(1200, 700);
            this.WindowState = FormWindowState.Maximized;
        }

        private void CreateHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = PrimaryColor
            };

            headerLabel = new Label
            {
                Text = $"Hoş geldiniz, {CurrentUser.User.AdSoyad} (Üye)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            logoutButton = new Button
            {
                Text = "🚪 ÇIKIŞ",
                Size = new Size(100, 30),
                Location = new Point(this.Width - 120, 15),
                FlatStyle = FlatStyle.Flat,
                BackColor = DangerColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.Click += LogoutButton_Click;

            headerPanel.Controls.AddRange(new Control[] { headerLabel, logoutButton });
            this.Controls.Add(headerPanel);
        }

        private void CreateSidebar()
        {
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            // Menü başlığı
            Label menuTitle = new Label
            {
                Text = "ÜYE MENÜSÜ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(210, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Menü butonları (Üye için kısıtlı)
            btnAnaSayfa = CreateMenuButton("🏠 Ana Sayfa", 70);
            btnAidatTakibi = CreateMenuButton("💰 Aidat Takibi", 120);
            btnEtkinlikler = CreateMenuButton("🎉 Etkinlikler", 170);
            btnAyarlar = CreateMenuButton("⚙️ Ayarlar", 220);

            // Event handlers
            btnAnaSayfa.Click += (s, e) => LoadPage(new MemberAnaSayfaPage());
            btnAidatTakibi.Click += (s, e) => LoadPage(new MemberAidatTakibiPage());
            btnEtkinlikler.Click += (s, e) => LoadPage(new MemberEtkinliklerPage());
            btnAyarlar.Click += (s, e) => LoadPage(new MemberAyarlarPage());

            sidebarPanel.Controls.AddRange(new Control[] {
                menuTitle, btnAnaSayfa, btnAidatTakibi, btnEtkinlikler, btnAyarlar
            });

            this.Controls.Add(sidebarPanel);

            // İlk butonu aktif yap
            SetActiveButton(btnAnaSayfa);
        }

        private Button CreateMenuButton(string text, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(210, 40),
                Location = new Point(20, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = AccentColor;

            return btn;
        }

        private void CreateContentArea()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = LightGray,
                Padding = new Padding(0)
            };

            this.Controls.Add(contentPanel);
        }

        private void LoadPage(BaseMenuPage page)
        {
            try
            {
                // Mevcut sayfayı temizle
                if (currentPage != null)
                {
                    contentPanel.Controls.Remove(currentPage);
                    currentPage.Dispose();
                }

                // Yeni sayfayı yükle
                currentPage = page;
                currentPage.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(currentPage);

                // Sayfayı yükle
                currentPage.LoadPage();

                // Menü butonunu aktif yap
                SetActiveButtonByPageType(page.GetType());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sayfa yüklenirken hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetActiveButtonByPageType(Type pageType)
        {
            Button activeButton = null;

            if (pageType == typeof(MemberAnaSayfaPage))
                activeButton = btnAnaSayfa;
            else if (pageType == typeof(MemberAidatTakibiPage))
                activeButton = btnAidatTakibi;
            else if (pageType == typeof(MemberEtkinliklerPage))
                activeButton = btnEtkinlikler;
            else if (pageType == typeof(MemberAyarlarPage))
                activeButton = btnAyarlar;

            if (activeButton != null)
                SetActiveButton(activeButton);
        }

        private void SetActiveButton(Button activeButton)
        {
            // Tüm butonları normal renge çevir
            foreach (Control control in sidebarPanel.Controls)
            {
                if (control is Button btn)
                {
                    // Sadece menü butonlarını kontrol et
                    if (btn.Text.Contains("Ana Sayfa") ||
                        btn.Text.Contains("Aidat") ||
                        btn.Text.Contains("Etkinlik") ||
                        btn.Text.Contains("Ayar"))
                    {
                        btn.BackColor = Color.Transparent;
                        btn.Font = new Font("Segoe UI", 11, FontStyle.Regular);
                    }
                }
            }

            // Aktif butonu vurgula
            activeButton.BackColor = AccentColor;
            activeButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        }
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Çıkış yapmak istediğinizden emin misiniz?",
                "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CurrentUser.Logout();
                this.Hide();

                LoginForm loginForm = new LoginForm();
                loginForm.FormClosed += (s, args) => this.Close();
                loginForm.Show();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Uygulamayı kapatmak istediğinizden emin misiniz?",
                    "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                CurrentUser.Logout();
                Application.Exit();
            }
            base.OnFormClosing(e);
        }
    }
}