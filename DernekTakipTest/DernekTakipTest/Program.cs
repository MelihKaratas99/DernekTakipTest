using System;
using System.Windows.Forms;

namespace DernekTakipSistemi
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // LoginForm ile başlat
            Application.Run(new LoginForm());
        }
    }
}