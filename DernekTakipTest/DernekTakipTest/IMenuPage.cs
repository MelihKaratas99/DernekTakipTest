using System;
using System.Drawing;
using System.Windows.Forms;

namespace DernekTakipSistemi.Pages
{
    /// <summary>
    /// Tüm menü sayfaları için temel interface
    /// </summary>
    public interface IMenuPage
    {
        string PageTitle { get; }
        string PageIcon { get; }
        void LoadPage();
        void RefreshPage();
    }

    /// <summary>
    /// Menü sayfaları için temel UserControl - Layout Düzeltilmiş
    /// </summary>
    public abstract class BaseMenuPage : UserControl, IMenuPage
    {
        // Modern renk paleti
        protected readonly Color PrimaryColor = Color.FromArgb(44, 62, 80);
        protected readonly Color AccentColor = Color.FromArgb(52, 152, 219);
        protected readonly Color SuccessColor = Color.FromArgb(39, 174, 96);
        protected readonly Color DangerColor = Color.FromArgb(231, 76, 60);
        protected readonly Color WarningColor = Color.FromArgb(243, 156, 18);
        protected readonly Color LightGray = Color.FromArgb(236, 240, 241);
        protected readonly Color DarkGray = Color.FromArgb(149, 165, 166);

        // Ana panel
        protected Panel MainContentPanel;
        protected Label PageTitleLabel;

        public abstract string PageTitle { get; }
        public abstract string PageIcon { get; }

        public BaseMenuPage()
        {
            InitializeBaseComponents();
            InitializePage();
        }

        private void InitializeBaseComponents()
        {
            // UserControl ayarları - Düzeltilmiş
            this.BackColor = LightGray;
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.Padding = new Padding(0);

            // Sayfa başlığı
            PageTitleLabel = new Label
            {
                Text = $"{PageIcon} {PageTitle}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 20),
                AutoSize = true
            };

            // Ana içerik paneli - Responsive
            MainContentPanel = new Panel
            {
                Location = new Point(30, 70),
                BackColor = LightGray,
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            this.Controls.AddRange(new Control[] { PageTitleLabel, MainContentPanel });

            // Resize event'i ekle
            this.Resize += BaseMenuPage_Resize;
            this.Load += BaseMenuPage_Load;
        }

        private void BaseMenuPage_Load(object sender, EventArgs e)
        {
            // İlk yüklendiğinde boyutları ayarla
            UpdatePanelSizes();
        }

        private void BaseMenuPage_Resize(object sender, EventArgs e)
        {
            // Boyut değiştiğinde panelleri güncelle
            UpdatePanelSizes();
        }

        private void UpdatePanelSizes()
        {
            if (MainContentPanel != null && this.Width > 0 && this.Height > 0)
            {
                MainContentPanel.Size = new Size(this.Width - 60, this.Height - 90);
            }
        }

        /// <summary>
        /// Alt sınıflar bu metodu override ederek sayfalarını oluşturur
        /// </summary>
        protected abstract void InitializePage();

        /// <summary>
        /// Sayfa yüklendiğinde çalışır
        /// </summary>
        public virtual void LoadPage()
        {
            // Override edilebilir
        }

        /// <summary>
        /// Sayfa yenilendiğinde çalışır
        /// </summary>
        public virtual void RefreshPage()
        {
            // Override edilebilir
        }

        // Yardımcı metodlar
        protected Button CreateActionButton(string text, Point location, Color backColor, EventHandler clickHandler = null)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(140, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            if (clickHandler != null)
                btn.Click += clickHandler;

            return btn;
        }

        protected Panel CreateContentPanel(Point location, Size size)
        {
            return new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
        }
    }
}