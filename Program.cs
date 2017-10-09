using System;
using System.Windows.Forms;

namespace Teleprompter {

    internal static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(String[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ////
            //args = new string[] { "C:\\Users\\Stephen\\Documents\\Projects\\Customers\\EasiQ\\Scripts\\ONTBYTSAKE RX25.7.14.esq" };
            Classes.Controller.RunProgram(args);
            Application.Run();
        }
    }
}