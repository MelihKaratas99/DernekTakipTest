using System;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages.Member
{
    public class MemberAidatTakibiPage : BaseMenuPage
    {
        public override string PageTitle => "Aidat Takibi";
        public override string PageIcon => "💰";

        private UyeService uyeService;
        private DataGridView aidatGeçmişiGrid;

        public MemberAidatTakibiPage()
        {
            uyeService = new UyeService();
        }

        protected override void InitializePage()
        {
            CreateAidatDurumuSection();
            CreateAidatGecmisiSection();
        }

        private void CreateAidatDurumuSection()
        {
            Panel durumPanel = CreateContentPanel(new Point(0, 0), new Size(970, 150));

            Label durumTitle = new Label
            {
                Text = "💳 Aidat Durumum",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Mevcut aidat bilgileri
            if (CurrentUser.User.UyeID.HasValue)
            {
                var uye = uyeService.UyeGetir(CurrentUser.User.UyeID.Value);
                if (uye != null)
                {
                    CreateAidatBilgileri(durumPanel, uye);
                }
            }

            durumPanel.Controls.Add(durumTitle);
            MainContentPanel.Controls.Add(durumPanel);
        }

        private void CreateAidatBilgileri(Panel parent, Uye uye)
        {
            // Borç durumu kartı
            Panel borcKarti = new Panel
            {
                Size = new Size(200, 80),
                Location = new Point(50, 50),
                BackColor = uye.AidatBorcu > 0 ? DangerColor : SuccessColor
            };

            Label borcIcon = new Label
            {
                Text = uye.AidatBorcu > 0 ? "⚠️" : "✅",
                Font = new Font("Segoe UI", 20),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                Size = new Size(30, 30)
            };

            Label borcBaslik = new Label
            {
                Text = "Toplam Borç",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 15),
                Size = new Size(140, 15)
            };

            Label borcTutar = new Label
            {
                Text = uye.AidatBorcu.ToString("C2"),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 35),
                Size = new Size(140, 25)
            };

            borcKarti.Controls.AddRange(new Control[] { borcIcon, borcBaslik, borcTutar });

            // Son ödeme kartı
            Panel odemeKarti = new Panel
            {
                Size = new Size(200, 80),
                Location = new Point(280, 50),
                BackColor = AccentColor
            };

            Label odemeIcon = new Label
            {
                Text = "📅",
                Font = new Font("Segoe UI", 20),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                Size = new Size(30, 30)
            };

            Label odemeBaslik = new Label
            {
                Text = "Son Ödeme",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 15),
                Size = new Size(140, 15)
            };

            Label odemeTarih = new Label
            {
                Text = uye.SonOdemeTarihi?.ToString("dd.MM.yyyy") ?? "Hiç",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 35),
                Size = new Size(140, 25)
            };

            odemeKarti.Controls.AddRange(new Control[] { odemeIcon, odemeBaslik, odemeTarih });

            // Ödeme butonu (eğer borç varsa)
            if (uye.AidatBorcu > 0)
            {
                Button odemeBtn = CreateActionButton("💳 ÖDEME YAP", new Point(520, 70), SuccessColor);
                odemeBtn.Click += OdemeBtn_Click;
                parent.Controls.Add(odemeBtn);
            }

            parent.Controls.AddRange(new Control[] { borcKarti, odemeKarti });
        }

        private void CreateAidatGecmisiSection()
        {
            Panel gecmisPanel = CreateContentPanel(new Point(0, 170), new Size(970, 380));

            Label gecmisTitle = new Label
            {
                Text = "📊 Aidat Ödemelerim",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // DataGrid oluştur
            aidatGeçmişiGrid = new DataGridView
            {
                Size = new Size(930, 320),
                Location = new Point(20, 45),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                Font = new Font("Segoe UI", 9),
                GridColor = Color.FromArgb(220, 220, 220),
                ScrollBars = ScrollBars.Both,
                AllowUserToResizeColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            ReflectionHelper.OptimizeDataGridView(aidatGeçmişiGrid);

            // Header stili
            aidatGeçmişiGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            aidatGeçmişiGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            aidatGeçmişiGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            aidatGeçmişiGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor;
            aidatGeçmişiGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            aidatGeçmişiGrid.ColumnHeadersHeight = 35;

            // Satır stili
            aidatGeçmişiGrid.DefaultCellStyle.BackColor = Color.White;
            aidatGeçmişiGrid.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            aidatGeçmişiGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            aidatGeçmişiGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            aidatGeçmişiGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            aidatGeçmişiGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            aidatGeçmişiGrid.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            aidatGeçmişiGrid.RowTemplate.Height = 30;

            // Alternatif satır rengi
            aidatGeçmişiGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            aidatGeçmişiGrid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            aidatGeçmişiGrid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Kolonları ekle
            aidatGeçmişiGrid.Columns.Add("Tarih", "TARİH");
            aidatGeçmişiGrid.Columns.Add("Tutar", "TUTAR");
            aidatGeçmişiGrid.Columns.Add("Aciklama", "AÇIKLAMA");
            aidatGeçmişiGrid.Columns.Add("Durum", "DURUM");

            // Fill Weight ile orantılı genişlikler
            aidatGeçmişiGrid.Columns[0].FillWeight = 25;  // Tarih %25
            aidatGeçmişiGrid.Columns[1].FillWeight = 20;  // Tutar %20
            aidatGeçmişiGrid.Columns[2].FillWeight = 40;  // Açıklama %40
            aidatGeçmişiGrid.Columns[3].FillWeight = 15;  // Durum %15

            // Kolon stilleri
            aidatGeçmişiGrid.Columns["Tarih"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            aidatGeçmişiGrid.Columns["Tutar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            aidatGeçmişiGrid.Columns["Tutar"].DefaultCellStyle.Format = "C2";
            aidatGeçmişiGrid.Columns["Durum"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            LoadAidatGecmisi();

            gecmisPanel.Controls.AddRange(new Control[] { gecmisTitle, aidatGeçmişiGrid });
            MainContentPanel.Controls.Add(gecmisPanel);
        }

        private void LoadAidatGecmisi()
        {
            try
            {
                aidatGeçmişiGrid.Rows.Clear();
                // Örnek aidat geçmişi verileri
                aidatGeçmişiGrid.Rows.Add("15.12.2024", "150.00", "Aralık 2024 Aidatı", "Ödendi");
                aidatGeçmişiGrid.Rows.Add("15.11.2024", "150.00", "Kasım 2024 Aidatı", "Ödendi");
                aidatGeçmişiGrid.Rows.Add("15.10.2024", "150.00", "Ekim 2024 Aidatı", "Ödendi");
                aidatGeçmişiGrid.Rows.Add("15.01.2025", "150.00", "Ocak 2025 Aidatı", "Beklemede");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aidat geçmişi yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OdemeBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ödeme sistemi entegrasyonu burada olacak.\n(Kredi kartı, havale, vs.)",
                "Ödeme", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override void LoadPage()
        {
            LoadAidatGecmisi();
        }

        public override void RefreshPage()
        {
            MainContentPanel.Controls.Clear();
            InitializePage();
        }
    }
}