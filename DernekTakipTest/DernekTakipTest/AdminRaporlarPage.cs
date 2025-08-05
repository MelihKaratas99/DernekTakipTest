using System;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages.Admin
{
    public class AdminRaporlarPage : BaseMenuPage
    {
        public override string PageTitle => "Raporlar";
        public override string PageIcon => "📊";

        protected override void InitializePage()
        {
            CreateReportSection();
        }

        private void CreateReportSection()
        {
            Panel reportsPanel = CreateContentPanel(new Point(0, 0), new Size(970, 500));

            Label reportsTitle = new Label
            {
                Text = "📋 Rapor Seçenekleri",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Rapor butonları
            Button uyeRaporuBtn = CreateActionButton("👥 Üye Raporu", new Point(50, 70), AccentColor);
            Button aidatRaporuBtn = CreateActionButton("💰 Aidat Raporu", new Point(220, 70), SuccessColor);
            Button etkinlikRaporuBtn = CreateActionButton("🎉 Etkinlik Raporu", new Point(390, 70), WarningColor);
            Button genelRaporBtn = CreateActionButton("📈 Genel Rapor", new Point(560, 70), DarkGray);

            // Event handlers
            uyeRaporuBtn.Click += (s, e) => MessageBox.Show("Üye raporu oluşturuluyor...", "Rapor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            aidatRaporuBtn.Click += (s, e) => MessageBox.Show("Aidat raporu oluşturuluyor...", "Rapor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            etkinlikRaporuBtn.Click += (s, e) => MessageBox.Show("Etkinlik raporu oluşturuluyor...", "Rapor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            genelRaporBtn.Click += (s, e) => MessageBox.Show("Genel rapor oluşturuluyor...", "Rapor", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Rapor açıklama metinleri
            Label uyeAciklama = new Label
            {
                Text = "Tüm üye bilgileri ve\nistatistikleri",
                Font = new Font("Segoe UI", 9),
                ForeColor = DarkGray,
                Location = new Point(50, 110),
                Size = new Size(140, 40),
                TextAlign = ContentAlignment.TopCenter
            };

            Label aidatAciklama = new Label
            {
                Text = "Aidat ödemeleri ve\nborç durumları",
                Font = new Font("Segoe UI", 9),
                ForeColor = DarkGray,
                Location = new Point(220, 110),
                Size = new Size(140, 40),
                TextAlign = ContentAlignment.TopCenter
            };

            Label etkinlikAciklama = new Label
            {
                Text = "Etkinlik katılımları\nve organizasyonlar",
                Font = new Font("Segoe UI", 9),
                ForeColor = DarkGray,
                Location = new Point(390, 110),
                Size = new Size(140, 40),
                TextAlign = ContentAlignment.TopCenter
            };

            Label genelAciklama = new Label
            {
                Text = "Genel istatistikler\nve özet bilgiler",
                Font = new Font("Segoe UI", 9),
                ForeColor = DarkGray,
                Location = new Point(560, 110),
                Size = new Size(140, 40),
                TextAlign = ContentAlignment.TopCenter
            };

            reportsPanel.Controls.AddRange(new Control[] {
                reportsTitle, uyeRaporuBtn, aidatRaporuBtn, etkinlikRaporuBtn, genelRaporBtn,
                uyeAciklama, aidatAciklama, etkinlikAciklama, genelAciklama
            });

            MainContentPanel.Controls.Add(reportsPanel);
        }

        public override void LoadPage()
        {
            // Rapor sayfası yüklendiğinde
        }

        public override void RefreshPage()
        {
            // Rapor sayfası yenilendiğinde
        }
    }
}