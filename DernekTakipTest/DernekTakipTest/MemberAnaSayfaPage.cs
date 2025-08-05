using System;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages.Member
{
    public class MemberAnaSayfaPage : BaseMenuPage
    {
        public override string PageTitle => "Ana Sayfa";
        public override string PageIcon => "🏠";

        private UyeService uyeService;

        public MemberAnaSayfaPage()
        {
            uyeService = new UyeService();
        }

        protected override void InitializePage()
        {
            CreateWelcomeSection();
            CreatePersonalInfoSection();
            CreateQuickInfoSection();
        }

        private void CreateWelcomeSection()
        {
            Panel welcomePanel = CreateContentPanel(new Point(0, 0), new Size(970, 80));

            Label welcomeLabel = new Label
            {
                Text = $"Hoş geldiniz, {CurrentUser.User.AdSoyad}!",
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

        private void CreatePersonalInfoSection()
        {
            Panel infoPanel = CreateContentPanel(new Point(0, 100), new Size(970, 200));

            Label infoTitle = new Label
            {
                Text = "👤 Kişisel Bilgilerim",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Üye bilgilerini getir (CurrentUser.User.UyeID ile)
            Uye currentUye = null;
            if (CurrentUser.User.UyeID.HasValue)
            {
                currentUye = uyeService.UyeGetir(CurrentUser.User.UyeID.Value);
            }

            if (currentUye != null)
            {
                CreateInfoLabels(infoPanel, currentUye);
            }
            else
            {
                Label noInfoLabel = new Label
                {
                    Text = "Üye bilgileri bulunamadı.",
                    Font = new Font("Segoe UI", 10),
                    ForeColor = DangerColor,
                    Location = new Point(20, 50),
                    AutoSize = true
                };
                infoPanel.Controls.Add(noInfoLabel);
            }

            infoPanel.Controls.Add(infoTitle);
            MainContentPanel.Controls.Add(infoPanel);
        }

        private void CreateInfoLabels(Panel parent, Uye uye)
        {
            // Sol kolon
            CreateInfoLabel(parent, "TC Kimlik:", uye.TC, new Point(30, 50));
            CreateInfoLabel(parent, "Ad Soyad:", uye.AdSoyad, new Point(30, 80));
            CreateInfoLabel(parent, "Telefon:", uye.Telefon, new Point(30, 110));
            CreateInfoLabel(parent, "Üyelik Tarihi:", uye.UyelikTarihi.ToString("dd.MM.yyyy"), new Point(30, 140));

            // Sağ kolon
            CreateInfoLabel(parent, "E-posta:", uye.Email ?? "Belirtilmemiş", new Point(500, 50));
            CreateInfoLabel(parent, "Durum:", uye.UyelikDurumu, new Point(500, 80));
            CreateInfoLabel(parent, "Aidat Borcu:", uye.AidatBorcu.ToString("C2"), new Point(500, 110));
            CreateInfoLabel(parent, "Son Ödeme:", uye.SonOdemeTarihi?.ToString("dd.MM.yyyy") ?? "Hiç", new Point(500, 140));
        }

        private void CreateInfoLabel(Panel parent, string title, string value, Point location)
        {
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = DarkGray,
                Location = location,
                Size = new Size(100, 20)
            };

            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 9),
                ForeColor = PrimaryColor,
                Location = new Point(location.X + 120, location.Y),
                Size = new Size(250, 20)
            };

            parent.Controls.AddRange(new Control[] { titleLabel, valueLabel });
        }

        private void CreateQuickInfoSection()
        {
            Panel quickPanel = CreateContentPanel(new Point(0, 320), new Size(970, 120));

            Label quickTitle = new Label
            {
                Text = "⚡ Hızlı Bilgiler",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Bilgi kartları
            CreateInfoCard(quickPanel, "📅", "Üyelik Süresi", CalculateMembershipDuration(), AccentColor, new Point(50, 50));
            CreateInfoCard(quickPanel, "💰", "Aidat Durumu", GetPaymentStatus(), GetPaymentStatusColor(), new Point(250, 50));
            CreateInfoCard(quickPanel, "🎉", "Etkinlik Sayısı", "5", SuccessColor, new Point(450, 50));
            CreateInfoCard(quickPanel, "📧", "Bildirimler", "2 Yeni", WarningColor, new Point(650, 50));

            quickPanel.Controls.Add(quickTitle);
            MainContentPanel.Controls.Add(quickPanel);
        }

        private void CreateInfoCard(Panel parent, string icon, string title, string value, Color color, Point location)
        {
            Panel card = new Panel
            {
                Size = new Size(180, 50),
                Location = location,
                BackColor = color
            };

            Label iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.White,
                Location = new Point(10, 5),
                Size = new Size(25, 20)
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 5),
                Size = new Size(130, 15)
            };

            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 22),
                Size = new Size(130, 20)
            };

            card.Controls.AddRange(new Control[] { iconLabel, titleLabel, valueLabel });
            parent.Controls.Add(card);
        }

        private string CalculateMembershipDuration()
        {
            if (CurrentUser.User.UyeID.HasValue)
            {
                var uye = uyeService.UyeGetir(CurrentUser.User.UyeID.Value);
                if (uye != null)
                {
                    var duration = DateTime.Now - uye.UyelikTarihi;
                    if (duration.Days > 365)
                        return $"{(int)(duration.Days / 365)} Yıl";
                    else if (duration.Days > 30)
                        return $"{(int)(duration.Days / 30)} Ay";
                    else
                        return $"{duration.Days} Gün";
                }
            }
            return "Bilinmiyor";
        }

        private string GetPaymentStatus()
        {
            if (CurrentUser.User.UyeID.HasValue)
            {
                var uye = uyeService.UyeGetir(CurrentUser.User.UyeID.Value);
                if (uye != null)
                {
                    return uye.AidatBorcu > 0 ? "Borçlu" : "Güncel";
                }
            }
            return "Bilinmiyor";
        }

        private Color GetPaymentStatusColor()
        {
            string status = GetPaymentStatus();
            return status == "Borçlu" ? DangerColor : SuccessColor;
        }

        public override void LoadPage()
        {
            // Sayfa yüklendiğinde
        }

        public override void RefreshPage()
        {
            MainContentPanel.Controls.Clear();
            InitializePage();
        }
    }
}