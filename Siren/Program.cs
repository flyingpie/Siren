using System;
using System.Windows.Forms;

namespace Siren
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //try
            //{
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Search());
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Woops, stuff is broken: " + e.Message, "Stuff is NOT nominal!!");
            //}
        }
    }
}