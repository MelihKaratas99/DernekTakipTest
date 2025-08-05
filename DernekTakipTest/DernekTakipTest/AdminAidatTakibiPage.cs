using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DernekTakipSistemi.Pages.Admin
{
    public class AdminAidatTakibiPage : BaseMenuPage
    {
        public override string PageTitle => "Aidat Takibi";
        public override string PageIcon => "💰";

        private DataGridView aidatDataGrid;
        private TextBox aramaTextBox;
        private Button odemeAlButton, borclulariGosterButton, refreshButton;
        private ComboBox durumComboBox;
        private UyeService uyeService;

        public AdminAidatTakibiPage()
        {
            uyeService = new UyeService();
        }

        protected override void InitializePage()
        {
            CreateFilterSection();
            CreateDataGridSection();
            LoadAidatlar();
        }

        private void CreateFilterSection()
        {
            Panel filterPanel = CreateContentPanel(new Point(0, 0), new Size(970, 60));

            Label searchLabel = new Label
            {
                Text = "🔍 Arama:",
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = DarkGray,
                AutoSize = true
            };

            aramaTextBox = new TextBox
            {
                Location = new Point(90, 17),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            aramaTextBox.TextChanged += AramaTextBox_TextChanged;

            Label durumLabel = new Label
            {
                Text = "Durum:",
                Location = new Point(260, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = DarkGray,
                AutoSize = true
            };

            durumComboBox = new ComboBox
            {
                Location = new Point(310, 17),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            durumComboBox.Items.AddRange(new string[] { "Tümü", "Borçlu", "Güncel" });
            durumComboBox.SelectedIndex = 0;
            durumComboBox.SelectedIndexChanged += DurumComboBox_SelectedIndexChanged;

            // İşlem butonları
            odemeAlButton = CreateActionButton("💳 ÖDEME AL", new Point(450, 15), SuccessColor);
            odemeAlButton.Click += OdemeAlButton_Click;

            borclulariGosterButton = CreateActionButton("⚠️ BORÇLULAR", new Point(610, 15), WarningColor);
            borclulariGosterButton.Click += BorclulariGosterButton_Click;

            refreshButton = CreateActionButton("🔄 YENİLE", new Point(770, 15), DarkGray);
            refreshButton.Click += RefreshButton_Click;

            filterPanel.Controls.AddRange(new Control[] {
                searchLabel, aramaTextBox, durumLabel, durumComboBox,
                odemeAlButton, borclulariGosterButton, refreshButton
            });

            MainContentPanel.Controls.Add(filterPanel);
        }

        private void CreateDataGridSection()
        {
            aidatDataGrid = new DataGridView
            {
                Size = new Size(970, 480),
                Location = new Point(0, 80),
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

            ReflectionHelper.OptimizeDataGridView(aidatDataGrid);

            // Header stili
            aidatDataGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            aidatDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            aidatDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            aidatDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor;
            aidatDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            aidatDataGrid.ColumnHeadersHeight = 35;

            // Satır stili
            aidatDataGrid.DefaultCellStyle.BackColor = Color.White;
            aidatDataGrid.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            aidatDataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            aidatDataGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            aidatDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            aidatDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            aidatDataGrid.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            aidatDataGrid.RowTemplate.Height = 30;

            // Alternatif satır rengi
            aidatDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            aidatDataGrid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            aidatDataGrid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Kolonları ekle
            aidatDataGrid.Columns.Add("UyeID", "ID");
            aidatDataGrid.Columns.Add("AdSoyad", "AD SOYAD");
            aidatDataGrid.Columns.Add("TC", "TC NO");
            aidatDataGrid.Columns.Add("AidatBorcu", "BORÇ TUTARI");
            aidatDataGrid.Columns.Add("SonOdemeTarihi", "SON ÖDEME");
            aidatDataGrid.Columns.Add("Durum", "DURUM");

            // Fill Weight ile orantılı genişlikler
            aidatDataGrid.Columns[0].Visible = false;  // ID gizli
            aidatDataGrid.Columns[1].FillWeight = 30;  // Ad Soyad %30
            aidatDataGrid.Columns[2].FillWeight = 20;  // TC %20
            aidatDataGrid.Columns[3].FillWeight = 20;  // Borç %20
            aidatDataGrid.Columns[4].FillWeight = 20;  // Son Ödeme %20
            aidatDataGrid.Columns[5].FillWeight = 10;  // Durum %10

            // Kolon stilleri
            aidatDataGrid.Columns["AidatBorcu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            aidatDataGrid.Columns["AidatBorcu"].DefaultCellStyle.Format = "C2";
            aidatDataGrid.Columns["SonOdemeTarihi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            aidatDataGrid.Columns["SonOdemeTarihi"].DefaultCellStyle.Format = "dd.MM.yyyy";
            aidatDataGrid.Columns["Durum"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Event handlers
            aidatDataGrid.CellFormatting += AidatDataGrid_CellFormatting;

            MainContentPanel.Controls.Add(aidatDataGrid);
        }

        private void LoadAidatlar()
        {
            try
            {
                aidatDataGrid.Rows.Clear();
                List<Uye> uyeler = uyeService.TumUyeleriGetir();

                foreach (Uye uye in uyeler)
                {
                    string durum = uye.AidatBorcu > 0 ? "Borçlu" : "Güncel";
                    string sonOdeme = uye.SonOdemeTarihi?.ToString("dd.MM.yyyy") ?? "Hiç";

                    aidatDataGrid.Rows.Add(
                        uye.UyeID,
                        uye.AdSoyad,
                        uye.TC,
                        uye.AidatBorcu,
                        sonOdeme,
                        durum
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aidat verileri yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AidatDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string value = e.Value.ToString();

            switch (aidatDataGrid.Columns[e.ColumnIndex].Name)
            {
                case "Durum":
                    if (value == "Borçlu")
                    {
                        e.CellStyle.ForeColor = DangerColor;
                        e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    }
                    else if (value == "Güncel")
                    {
                        e.CellStyle.ForeColor = SuccessColor;
                        e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    }
                    break;
            }
        }

        // Event Handlers
        private void OdemeAlButton_Click(object sender, EventArgs e)
        {
            if (aidatDataGrid.SelectedRows.Count > 0)
            {
                MessageBox.Show("Ödeme alma formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen ödeme alınacak üyeyi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BorclulariGosterButton_Click(object sender, EventArgs e)
        {
            durumComboBox.SelectedIndex = 1; // Borçlu seçeneğini aktif et
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadAidatlar();
            aramaTextBox.Clear();
            durumComboBox.SelectedIndex = 0;
        }

        private void AramaTextBox_TextChanged(object sender, EventArgs e)
        {
            // Arama işlemi
            LoadAidatlar(); // Basit yenileme - geliştirilecek
        }

        private void DurumComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filtreleme işlemi
            LoadAidatlar(); // Basit yenileme - geliştirilecek
        }

        public override void LoadPage()
        {
            LoadAidatlar();
        }

        public override void RefreshPage()
        {
            LoadAidatlar();
        }
    }
}