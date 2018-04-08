using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonPFE
{
    
    public enum ExitCode : int
    {
        Success = 0,
        UnknownError = 1
    }

    public enum ConnectivityState : int
    {
        Offline = 0,
        Online = 1,
    }

    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
