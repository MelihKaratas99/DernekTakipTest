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
    /// Menü sayfaları için temel UserControl
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
            // UserControl ayarları
            this.Size = new Size(1030, 660);
            this.BackColor = LightGray;
            this.Dock = DockStyle.Fill;

            // Sayfa başlığı
            PageTitleLabel = new Label
            {
                Text = $"{PageIcon} {PageTitle}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Location = new Point(30, 20),
                Size = new Size(970, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Ana içerik paneli
            MainContentPanel = new Panel
            {
                Location = new Point(30, 70),
                Size = new Size(970, 570),
                BackColor = LightGray,
                AutoScroll = true
            };

            this.Controls.AddRange(new Control[] { PageTitleLabel, MainContentPanel });
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
                BorderStyle = BorderStyle.FixedSingle
            };
        }
    }
}