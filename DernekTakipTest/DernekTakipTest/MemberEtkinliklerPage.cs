using System;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages.Member
{
    public class MemberEtkinliklerPage : BaseMenuPage
    {
        public override string PageTitle => "Etkinlikler";
        public override string PageIcon => "🎉";

        private DataGridView etkinliklerGrid;

        protected override void InitializePage()
        {
            CreateEtkinliklerSection();
        }

        private void CreateEtkinliklerSection()
        {
            Panel etkinlikPanel = CreateContentPanel(new Point(0, 0), new Size(970, 520));

            Label etkinlikTitle = new Label
            {
                Text = "🎯 Etkinlikler",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // DataGrid oluştur
            etkinliklerGrid = new DataGridView
            {
                Size = new Size(930, 460),
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

            ReflectionHelper.OptimizeDataGridView(etkinliklerGrid);

            // Header stili
            etkinliklerGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            etkinliklerGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            etkinliklerGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            etkinliklerGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor;
            etkinliklerGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            etkinliklerGrid.ColumnHeadersHeight = 35;

            // Satır stili
            etkinliklerGrid.DefaultCellStyle.BackColor = Color.White;
            etkinliklerGrid.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            etkinliklerGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            etkinliklerGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            etkinliklerGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            etkinliklerGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            etkinliklerGrid.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            etkinliklerGrid.RowTemplate.Height = 30;

            // Alternatif satır rengi
            etkinliklerGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            etkinliklerGrid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            etkinliklerGrid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Kolonları ekle
            etkinliklerGrid.Columns.Add("EtkinlikAdi", "ETKİNLİK ADI");
            etkinliklerGrid.Columns.Add("Tarih", "TARİH");
            etkinliklerGrid.Columns.Add("Konum", "KONUM");
            etkinliklerGrid.Columns.Add("Aciklama", "AÇIKLAMA");
            etkinliklerGrid.Columns.Add("KatilimDurumu", "KATILIM");

            // Fill Weight ile orantılı genişlikler
            etkinliklerGrid.Columns[0].FillWeight = 25;  // Etkinlik Adı %25
            etkinliklerGrid.Columns[1].FillWeight = 15;  // Tarih %15
            etkinliklerGrid.Columns[2].FillWeight = 20;  // Konum %20
            etkinliklerGrid.Columns[3].FillWeight = 30;  // Açıklama %30
            etkinliklerGrid.Columns[4].FillWeight = 10;  // Katılım %10

            // Kolon stilleri
            etkinliklerGrid.Columns["Tarih"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            etkinliklerGrid.Columns["KatilimDurumu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Event handlers
            etkinliklerGrid.CellFormatting += EtkinliklerGrid_CellFormatting;
            etkinliklerGrid.CellDoubleClick += EtkinliklerGrid_CellDoubleClick;

            LoadEtkinlikler();

            etkinlikPanel.Controls.AddRange(new Control[] { etkinlikTitle, etkinliklerGrid });
            MainContentPanel.Controls.Add(etkinlikPanel);
        }

        private void LoadEtkinlikler()
        {
            try
            {
                etkinliklerGrid.Rows.Clear();

                // Örnek etkinlik verileri
                etkinliklerGrid.Rows.Add("Yılbaşı Gecesi", "31.12.2024", "Dernek Merkezi", "Geleneksel yılbaşı kutlaması", "Katılacak");
                etkinliklerGrid.Rows.Add("Bahar Gezisi", "15.04.2025", "Şile", "Doğa yürüyüşü ve piknik", "Katılmadı");
                etkinliklerGrid.Rows.Add("Seminer", "20.05.2025", "Online", "Kişisel gelişim semineri", "Katılacak");
                etkinliklerGrid.Rows.Add("Spor Turnuvası", "10.06.2025", "Spor Salonu", "Halı saha futbol turnuvası", "Beklemede");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Etkinlik verileri yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EtkinliklerGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string value = e.Value.ToString();

            switch (etkinliklerGrid.Columns[e.ColumnIndex].Name)
            {
                case "KatilimDurumu":
                    switch (value)
                    {
                        case "Katılacak":
                            e.CellStyle.ForeColor = SuccessColor;
                            e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                            break;
                        case "Katılmadı":
                            e.CellStyle.ForeColor = DangerColor;
                            e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                            break;
                        case "Beklemede":
                            e.CellStyle.ForeColor = WarningColor;
                            e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                            break;
                    }
                    break;
            }
        }

        private void EtkinliklerGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string etkinlikAdi = etkinliklerGrid.Rows[e.RowIndex].Cells["EtkinlikAdi"].Value.ToString();
                string tarih = etkinliklerGrid.Rows[e.RowIndex].Cells["Tarih"].Value.ToString();
                string konum = etkinliklerGrid.Rows[e.RowIndex].Cells["Konum"].Value.ToString();
                string aciklama = etkinliklerGrid.Rows[e.RowIndex].Cells["Aciklama"].Value.ToString();

                string detay = $"Etkinlik: {etkinlikAdi}\n" +
                              $"Tarih: {tarih}\n" +
                              $"Konum: {konum}\n" +
                              $"Açıklama: {aciklama}\n\n" +
                              $"Katılım durumunuzu değiştirmek istiyor musunuz?";

                DialogResult result = MessageBox.Show(detay, "Etkinlik Detayları",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    // Katılım durumu değiştirme işlemi
                    MessageBox.Show("Katılım durumu güncellendi.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public override void LoadPage()
        {
            LoadEtkinlikler();
        }

        public override void RefreshPage()
        {
            LoadEtkinlikler();
        }
    }
}