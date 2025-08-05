using System;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages.Admin
{
    public class AdminEtkinliklerPage : BaseMenuPage
    {
        public override string PageTitle => "Etkinlikler";
        public override string PageIcon => "🎉";

        private DataGridView etkinliklerDataGrid;
        private Button addEventButton, editEventButton, deleteEventButton, refreshButton;

        protected override void InitializePage()
        {
            CreateActionSection();
            CreateDataGridSection();
            LoadEtkinlikler();
        }

        private void CreateActionSection()
        {
            Panel actionPanel = CreateContentPanel(new Point(0, 0), new Size(970, 60));

            Label titleLabel = new Label
            {
                Text = "🎯 Etkinlik Yönetimi",
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                AutoSize = true
            };

            // İşlem butonları
            addEventButton = CreateActionButton("+ YENİ ETKİNLİK", new Point(250, 15), SuccessColor);
            addEventButton.Click += AddEventButton_Click;

            editEventButton = CreateActionButton("✏️ DÜZENLE", new Point(410, 15), AccentColor);
            editEventButton.Click += EditEventButton_Click;

            deleteEventButton = CreateActionButton("🗑️ SİL", new Point(570, 15), DangerColor);
            deleteEventButton.Click += DeleteEventButton_Click;

            refreshButton = CreateActionButton("🔄 YENİLE", new Point(730, 15), DarkGray);
            refreshButton.Click += RefreshButton_Click;

            actionPanel.Controls.AddRange(new Control[] {
                titleLabel, addEventButton, editEventButton, deleteEventButton, refreshButton
            });

            MainContentPanel.Controls.Add(actionPanel);
        }

        private void CreateDataGridSection()
        {
            etkinliklerDataGrid = new DataGridView
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

            ReflectionHelper.OptimizeDataGridView(etkinliklerDataGrid);

            // Header stili
            etkinliklerDataGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            etkinliklerDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            etkinliklerDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            etkinliklerDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor;
            etkinliklerDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            etkinliklerDataGrid.ColumnHeadersHeight = 35;

            // Satır stili
            etkinliklerDataGrid.DefaultCellStyle.BackColor = Color.White;
            etkinliklerDataGrid.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            etkinliklerDataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            etkinliklerDataGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            etkinliklerDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            etkinliklerDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            etkinliklerDataGrid.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            etkinliklerDataGrid.RowTemplate.Height = 30;

            // Alternatif satır rengi
            etkinliklerDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            etkinliklerDataGrid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            etkinliklerDataGrid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Kolonları ekle
            etkinliklerDataGrid.Columns.Add("EtkinlikID", "ID");
            etkinliklerDataGrid.Columns.Add("EtkinlikAdi", "ETKİNLİK ADI");
            etkinliklerDataGrid.Columns.Add("Tarih", "TARİH");
            etkinliklerDataGrid.Columns.Add("Konum", "KONUM");
            etkinliklerDataGrid.Columns.Add("KatilimciSayisi", "KATILIMCI");
            etkinliklerDataGrid.Columns.Add("Durum", "DURUM");

            // Fill Weight ile orantılı genişlikler
            etkinliklerDataGrid.Columns[0].Visible = false;  // ID gizli
            etkinliklerDataGrid.Columns[1].FillWeight = 35;  // Etkinlik Adı %35
            etkinliklerDataGrid.Columns[2].FillWeight = 20;  // Tarih %20
            etkinliklerDataGrid.Columns[3].FillWeight = 25;  // Konum %25
            etkinliklerDataGrid.Columns[4].FillWeight = 10;  // Katılımcı %10
            etkinliklerDataGrid.Columns[5].FillWeight = 10;  // Durum %10

            // Kolon stilleri
            etkinliklerDataGrid.Columns["Tarih"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            etkinliklerDataGrid.Columns["KatilimciSayisi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            etkinliklerDataGrid.Columns["Durum"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            MainContentPanel.Controls.Add(etkinliklerDataGrid);
        }

        private void LoadEtkinlikler()
        {
            try
            {
                etkinliklerDataGrid.Rows.Clear();

                // Örnek etkinlik verileri
                etkinliklerDataGrid.Rows.Add(1, "Yılbaşı Gecesi", "31.12.2024", "Dernek Merkezi", "45", "Planlanıyor");
                etkinliklerDataGrid.Rows.Add(2, "Bahar Gezisi", "15.04.2025", "Şile", "32", "Tamamlandı");
                etkinliklerDataGrid.Rows.Add(3, "Seminer", "20.05.2025", "Online", "78", "Planlanıyor");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Etkinlik verileri yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event Handlers
        private void AddEventButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Yeni etkinlik ekleme formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditEventButton_Click(object sender, EventArgs e)
        {
            if (etkinliklerDataGrid.SelectedRows.Count > 0)
            {
                MessageBox.Show("Etkinlik düzenleme formu burada açılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen düzenlenecek etkinliği seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteEventButton_Click(object sender, EventArgs e)
        {
            if (etkinliklerDataGrid.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Bu etkinliği silmek istediğinizden emin misiniz?",
                    "Etkinlik Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Etkinlik silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEtkinlikler();
                }
            }
            else
            {
                MessageBox.Show("Lütfen silinecek etkinliği seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadEtkinlikler();
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