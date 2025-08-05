using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DernekTakipSistemi.Pages.Admin
{
    public class AdminUyeYonetimiPage : BaseMenuPage
    {
        public override string PageTitle => "Üye Yönetimi";
        public override string PageIcon => "👥";

        private DataGridView uyelerDataGrid;
        private TextBox aramaTextBox;
        private Button addButton, editButton, deleteButton, refreshButton;
        private UyeService uyeService;

        public AdminUyeYonetimiPage()
        {
            uyeService = new UyeService();
        }

        protected override void InitializePage()
        {
            CreateSearchSection();
            CreateDataGridSection();
            LoadUyeler();
        }

        private void CreateSearchSection()
        {
            Panel searchPanel = CreateContentPanel(new Point(0, 0), new Size(970, 60));

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
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            aramaTextBox.TextChanged += AramaTextBox_TextChanged;

            // İşlem butonları
            addButton = CreateActionButton("+ YENİ ÜYE", new Point(320, 15), SuccessColor);
            addButton.Click += AddButton_Click;

            editButton = CreateActionButton("✏️ DÜZENLE", new Point(480, 15), AccentColor);
            editButton.Click += EditButton_Click;

            deleteButton = CreateActionButton("🗑️ SİL", new Point(640, 15), DangerColor);
            deleteButton.Click += DeleteButton_Click;

            refreshButton = CreateActionButton("🔄 YENİLE", new Point(800, 15), DarkGray);
            refreshButton.Click += RefreshButton_Click;

            searchPanel.Controls.AddRange(new Control[] {
                searchLabel, aramaTextBox, addButton, editButton, deleteButton, refreshButton
            });

            MainContentPanel.Controls.Add(searchPanel);
        }

        private void CreateDataGridSection()
        {
            uyelerDataGrid = new DataGridView
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

            // Reflection ile performans optimizasyonu
            ReflectionHelper.OptimizeDataGridView(uyelerDataGrid);

            // Header stili
            uyelerDataGrid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            uyelerDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            uyelerDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            uyelerDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PrimaryColor;
            uyelerDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            uyelerDataGrid.ColumnHeadersHeight = 35;

            // Satır stili
            uyelerDataGrid.DefaultCellStyle.BackColor = Color.White;
            uyelerDataGrid.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            uyelerDataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            uyelerDataGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            uyelerDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            uyelerDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            uyelerDataGrid.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            uyelerDataGrid.RowTemplate.Height = 30;

            // Alternatif satır rengi
            uyelerDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            uyelerDataGrid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            uyelerDataGrid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Kolonları ekle
            uyelerDataGrid.Columns.Add("UyeID", "ID");
            uyelerDataGrid.Columns.Add("TC", "TC NO");
            uyelerDataGrid.Columns.Add("AdSoyad", "AD SOYAD");
            uyelerDataGrid.Columns.Add("Telefon", "TELEFON");
            uyelerDataGrid.Columns.Add("Email", "E-MAIL");
            uyelerDataGrid.Columns.Add("UyelikDurumu", "DURUM");

            // Fill Weight ile orantılı genişlikler
            uyelerDataGrid.Columns[0].Visible = false;  // ID gizli
            uyelerDataGrid.Columns[1].FillWeight = 15;  // TC %15
            uyelerDataGrid.Columns[2].FillWeight = 30;  // Ad Soyad %30
            uyelerDataGrid.Columns[3].FillWeight = 20;  // Telefon %20
            uyelerDataGrid.Columns[4].FillWeight = 25;  // Email %25
            uyelerDataGrid.Columns[5].FillWeight = 10;  // Durum %10

            // Kolon stilleri
            uyelerDataGrid.Columns["TC"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            uyelerDataGrid.Columns["Telefon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            uyelerDataGrid.Columns["UyelikDurumu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Event handlers
            uyelerDataGrid.CellFormatting += UyelerDataGrid_CellFormatting;

            MainContentPanel.Controls.Add(uyelerDataGrid);
        }

        private void LoadUyeler()
        {
            try
            {
                uyelerDataGrid.Rows.Clear();
                List<Uye> uyeler = uyeService.TumUyeleriGetir();

                foreach (Uye uye in uyeler)
                {
                    uyelerDataGrid.Rows.Add(
                        uye.UyeID,
                        uye.TC,
                        uye.AdSoyad,
                        uye.Telefon,
                        uye.Email ?? "",
                        uye.UyelikDurumu
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UyelerDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string value = e.Value.ToString();

            switch (uyelerDataGrid.Columns[e.ColumnIndex].Name)
            {
                case "UyelikDurumu":
                    if (value == "Aktif")
                    {
                        e.CellStyle.ForeColor = SuccessColor;
                        e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    }
                    else if (value == "Pasif")
                    {
                        e.CellStyle.ForeColor = DangerColor;
                        e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    }
                    break;
            }
        }

        // Event Handlers
        private void AddButton_Click(object sender, EventArgs e)
        {
            UyeEkleDuzenleForm form = new UyeEkleDuzenleForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadUyeler();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (uyelerDataGrid.SelectedRows.Count > 0)
            {
                int uyeID = (int)uyelerDataGrid.SelectedRows[0].Cells["UyeID"].Value;
                Uye uye = uyeService.UyeGetir(uyeID);

                if (uye != null)
                {
                    UyeEkleDuzenleForm form = new UyeEkleDuzenleForm(uye);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadUyeler();
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen düzenlenecek üyeyi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (uyelerDataGrid.SelectedRows.Count > 0)
            {
                int uyeID = (int)uyelerDataGrid.SelectedRows[0].Cells["UyeID"].Value;

                if (uyeService.UyeSil(uyeID))
                {
                    LoadUyeler();
                }
            }
            else
            {
                MessageBox.Show("Lütfen silinecek üyeyi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadUyeler();
            aramaTextBox.Clear();
        }

        private void AramaTextBox_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = aramaTextBox.Text.Trim();

            if (string.IsNullOrEmpty(aramaMetni))
            {
                LoadUyeler();
            }
            else
            {
                uyelerDataGrid.Rows.Clear();
                List<Uye> sonuclar = uyeService.UyeAra(aramaMetni);

                foreach (Uye uye in sonuclar)
                {
                    uyelerDataGrid.Rows.Add(
                        uye.UyeID,
                        uye.TC,
                        uye.AdSoyad,
                        uye.Telefon,
                        uye.Email ?? "",
                        uye.UyelikDurumu
                    );
                }
            }
        }

        public override void LoadPage()
        {
            LoadUyeler();
        }

        public override void RefreshPage()
        {
            LoadUyeler();
        }
    }
}