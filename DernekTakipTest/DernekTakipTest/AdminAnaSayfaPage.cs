using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DernekTakipSistemi.Pages.Admin
{
    public class AdminAnaSayfaPage : BaseMenuPage
    {
        public override string PageTitle => "Ana Sayfa";
        public override string PageIcon => "🏠";

        private UyeService uyeService;

        public AdminAnaSayfaPage()
        {
            uyeService = new UyeService();
        }

        protected override void InitializePage()
        {
            CreateWelcomeSection();
            CreateStatsSection();
            CreateQuickActionsSection();
        }

        private void CreateWelcomeSection()
        {
            Panel welcomePanel = CreateContentPanel(new Point(0, 0), new Size(950, 80));

            Label welcomeLabel = new Label
            {
                Text = $"Hoş geldiniz, {CurrentUser.User.AdSoyad}! (Yönetici)",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label dateLabel = new Label
            {
                Text = $"Bugün: {DateTime.Now:dd MMMM yyyy, dddd}",
                Font = new Font("Segoe UI", 10),
                ForeColor = DarkGray,
                Location = new Point(20, 45),
                AutoSize = true
            };

            welcomePanel.Controls.AddRange(new Control[] { welcomeLabel, dateLabel });
            MainContentPanel.Controls.Add(welcomePanel);
        }


        private void CreateStatsSection()
        {
            Panel statsPanel = CreateContentPanel(new Point(0, 100), new Size(950, 120));

            Label statsTitle = new Label
            {
                Text = "📊 İstatistikler",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // İstatistik kartları
            CreateStatCards(statsPanel);

            statsPanel.Controls.Add(statsTitle);
            MainContentPanel.Controls.Add(statsPanel);
        }


        private void CreateStatCards(Panel parent)
        {
            try
            {
                // UyeService'den güvenli istatistikler al
                int toplamUye = uyeService.ToplamUyeSayisi();
                int aktifUye = uyeService.AktifUyeSayisi();
                int pasifUye = toplamUye - aktifUye;
                int borcluUye = uyeService.BorcluUyeSayisi();

                CreateStatCard(parent, "👥", "Toplam Üye", toplamUye.ToString(), AccentColor, new Point(20, 50));
                CreateStatCard(parent, "✅", "Aktif Üye", aktifUye.ToString(), SuccessColor, new Point(200, 50));
                CreateStatCard(parent, "⏸️", "Pasif Üye", pasifUye.ToString(), DangerColor, new Point(380, 50));
                CreateStatCard(parent, "⚠️", "Borçlu Üye", borcluUye.ToString(), WarningColor, new Point(560, 50));
            }
            catch (Exception ex)
            {
                // Hata durumunda varsayılan kartlar
                CreateStatCard(parent, "❌", "Hata", "Veri yok", DangerColor, new Point(20, 50));
                CreateStatCard(parent, "❌", "Hata", "Veri yok", DangerColor, new Point(200, 50));
                CreateStatCard(parent, "❌", "Hata", "Veri yok", DangerColor, new Point(380, 50));
                CreateStatCard(parent, "❌", "Hata", "Veri yok", DangerColor, new Point(560, 50));

                System.Diagnostics.Debug.WriteLine($"İstatistik hatası: {ex.Message}");
            }
        }

        private void CreateStatCard(Panel parent, string icon, string title, string value, Color color, Point location)
        {
            Panel card = new Panel
            {
                Size = new Size(160, 60),
                Location = location,
                BackColor = color
            };

            Label iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 16),
                ForeColor = Color.White,
                Location = new Point(10, 8),
                Size = new Size(30, 25)
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(45, 8),
                Size = new Size(100, 15)
            };

            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(45, 25),
                Size = new Size(100, 25)
            };

            card.Controls.AddRange(new Control[] { iconLabel, titleLabel, valueLabel });
            parent.Controls.Add(card);
        }

        private void CreateQuickActionsSection()
        {
            Panel actionsPanel = CreateContentPanel(new Point(0, 240), new Size(950, 150));

            Label actionsTitle = new Label
            {
                Text = "⚡ Hızlı İşlemler",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Hızlı işlem butonları
            Button newMemberBtn = CreateActionButton("👤 Yeni Üye Ekle", new Point(20, 50), SuccessColor);
            Button viewReportsBtn = CreateActionButton("📊 Raporları Görüntüle", new Point(180, 50), AccentColor);
            Button managePaymentsBtn = CreateActionButton("💰 Aidat Yönetimi", new Point(340, 50), WarningColor);
            Button settingsBtn = CreateActionButton("⚙️ Sistem Ayarları", new Point(500, 50), DarkGray);

            actionsPanel.Controls.AddRange(new Control[] {
        actionsTitle, newMemberBtn, viewReportsBtn, managePaymentsBtn, settingsBtn
    });

            MainContentPanel.Controls.Add(actionsPanel);
        }

        public override void LoadPage()
        {
            // Sayfa her yüklendiğinde istatistikleri yenile
            RefreshPage();
        }

        public override void RefreshPage()
        {
            // İstatistikleri yeniden yükle
            MainContentPanel.Controls.Clear();
            InitializePage();
        }
    }
}