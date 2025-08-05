using System;
using System.Reflection;
using System.Windows.Forms;

namespace DernekTakipSistemi
{
    /// <summary>
    /// DataGridView performans optimizasyonu için yardımcı sınıf
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// DataGridView performansını optimize eder
        /// </summary>
        public static void OptimizeDataGridView(DataGridView dataGridView)
        {
            try
            {
                // DoubleBuffered özelliğini true yap (flickering'i önler)
                typeof(DataGridView).InvokeMember("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, dataGridView, new object[] { true });

                // Diğer performans ayarları
                dataGridView.EnableHeadersVisualStyles = false;
                dataGridView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            }
            catch (Exception ex)
            {
                // Hata durumunda sessizce devam et
                System.Diagnostics.Debug.WriteLine($"DataGridView optimizasyon hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Herhangi bir control için DoubleBuffered özelliğini aktif eder
        /// </summary>
        public static void SetDoubleBuffered(Control control, bool value)
        {
            try
            {
                PropertyInfo doubleBuffered = typeof(Control).GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                doubleBuffered?.SetValue(control, value, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DoubleBuffered ayarlama hatası: {ex.Message}");
            }
        }
    }
}