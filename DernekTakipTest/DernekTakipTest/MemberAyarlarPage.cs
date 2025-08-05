using System;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages.Member
{
    public class MemberAyarlarPage : BaseMenuPage
    {
        public override string PageTitle => "Ayarlar";
        public override string PageIcon => "⚙️";

        protected override void InitializePage()
        {
            CreatePersonalSettingsSection();
            CreatePasswordSection();
            CreateNotificationSection();
        }

        private void CreatePersonalSettingsSection()
        {
            Panel personalPanel = CreateContentPanel(new Point(0, 0), new Size(970, 150));

            Label personalTitle = new Label
            {
                Text = "👤 Kişisel Ayarlar",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Button profilGuncelleBtn = CreateActionButton("✏️ Profil Güncelle", new Point(50, 60), AccentColor);
            Button iletisimGuncelleBtn = CreateActionButton("📞 İletişim Güncelle", new Point(220, 60), SuccessColor);
            Button adresGuncelleBtn = CreateActionButton("🏠 Adres Güncelle", new Point(390, 60), WarningColor);

            profilGuncelleBtn.Click += (s, e) => MessageBox.Show("Profil güncelleme formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            iletisimGuncelleBtn.Click += (s, e) => MessageBox.Show("İletişim güncelleme formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            adresGuncelleBtn.Click += (s, e) => MessageBox.Show("Adres güncelleme formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            personalPanel.Controls.AddRange(new Control[] {
                personalTitle, profilGuncelleBtn, iletisimGuncelleBtn, adresGuncelleBtn
            });

            MainContentPanel.Controls.Add(personalPanel);
        }

        private void CreatePasswordSection()
        {
            Panel passwordPanel = CreateContentPanel(new Point(0, 170), new Size(970, 150));

            Label passwordTitle = new Label
            {
                Text = "🔒 Güvenlik Ayarları",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Button sifreDegistirBtn = CreateActionButton("🔑 Şifre Değiştir", new Point(50, 60), DangerColor);
            Button guvenlikSoruBtn = CreateActionButton("❓ Güvenlik Sorusu", new Point(220, 60), AccentColor);

            sifreDegistirBtn.Click += SifreDegistirBtn_Click;
            guvenlikSoruBtn.Click += (s, e) => MessageBox.Show("Güvenlik sorusu ayarlama formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            passwordPanel.Controls.AddRange(new Control[] {
                passwordTitle, sifreDegistirBtn, guvenlikSoruBtn
            });

            MainContentPanel.Controls.Add(passwordPanel);
        }

        private void CreateNotificationSection()
        {
            Panel notificationPanel = CreateContentPanel(new Point(0, 340), new Size(970, 150));

            Label notificationTitle = new Label
            {
                Text = "🔔 Bildirim Ayarları",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            CheckBox emailBildirimCb = new CheckBox
            {
                Text = "E-posta bildirimleri",
                Font = new Font("Segoe UI", 10),
                ForeColor = PrimaryColor,
                Location = new Point(50, 60),
                Checked = true,
                AutoSize = true
            };

            CheckBox smsBildirimCb = new CheckBox
            {
                Text = "SMS bildirimleri",
                Font = new Font("Segoe UI", 10),
                ForeColor = PrimaryColor,
                Location = new Point(50, 90),
                Checked = false,
                AutoSize = true
            };

            CheckBox etkinlikBildirimCb = new CheckBox
            {
                Text = "Etkinlik bildirimleri",
                Font = new Font("Segoe UI", 10),
                ForeColor = PrimaryColor,
                Location = new Point(250, 60),
                Checked = true,
                AutoSize = true
            };

            CheckBox aidatBildirimCb = new CheckBox
            {
                Text = "Aidat hatırlatmaları",
                Font = new Font("Segoe UI", 10),
                ForeColor = PrimaryColor,
                Location = new Point(250, 90),
                Checked = true,
                AutoSize = true
            };

            Button kaydetBtn = CreateActionButton("💾 KAYDET", new Point(450, 70), SuccessColor);
            kaydetBtn.Click += (s, e) => MessageBox.Show("Bildirim ayarları kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            notificationPanel.Controls.AddRange(new Control[] {
                notificationTitle, emailBildirimCb, smsBildirimCb, etkinlikBildirimCb, aidatBildirimCb, kaydetBtn
            });

            MainContentPanel.Controls.Add(notificationPanel);
        }

        private void SifreDegistirBtn_Click(object sender, EventArgs e)
        {
            // Basit şifre değiştirme formu
            Form sifreForm = new Form
            {
                Text = "Şifre Değiştir",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label eskiSifreLabel = new Label
            {
                Text = "Mevcut Şifre:",
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            TextBox eskiSifreTextBox = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(340, 25),
                UseSystemPasswordChar = true
            };

            Label yeniSifreLabel = new Label
            {
                Text = "Yeni Şifre:",
                Location = new Point(20, 80),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            TextBox yeniSifreTextBox = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(340, 25),
                UseSystemPasswordChar = true
            };

            Label sifreTekrarLabel = new Label
            {
                Text = "Yeni Şifre (Tekrar):",
                Location = new Point(20, 140),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            TextBox sifreTekrarTextBox = new TextBox
            {
                Location = new Point(20, 165),
                Size = new Size(340, 25),
                UseSystemPasswordChar = true
            };

            Button kaydetButton = new Button
            {
                Text = "KAYDET",
                Location = new Point(180, 210),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = SuccessColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            kaydetButton.FlatAppearance.BorderSize = 0;

            Button iptalButton = new Button
            {
                Text = "İPTAL",
                Location = new Point(290, 210),
                Size = new Size(70, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = DangerColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            iptalButton.FlatAppearance.BorderSize = 0;

            kaydetButton.Click += (s, args) =>
            {
                if (string.IsNullOrEmpty(eskiSifreTextBox.Text) ||
                    string.IsNullOrEmpty(yeniSifreTextBox.Text) ||
                    string.IsNullOrEmpty(sifreTekrarTextBox.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (yeniSifreTextBox.Text != sifreTekrarTextBox.Text)
                {
                    MessageBox.Show("Yeni şifreler eşleşmiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (eskiSifreTextBox.Text != CurrentUser.User.Sifre)
                {
                    MessageBox.Show("Mevcut şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Şifre güncelleme işlemi burada yapılacak
                MessageBox.Show("Şifreniz başarıyla değiştirildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sifreForm.Close();
            };

            iptalButton.Click += (s, args) => sifreForm.Close();

            sifreForm.Controls.AddRange(new Control[] {
                eskiSifreLabel, eskiSifreTextBox, yeniSifreLabel, yeniSifreTextBox,
                sifreTekrarLabel, sifreTekrarTextBox, kaydetButton, iptalButton
            });

            sifreForm.ShowDialog();
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