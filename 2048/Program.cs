using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _2048_Grafos
{
    static class Program
    {
        /// <summary>
        /// Entrada principal da aplicação.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Jogo());
        }
    }
}
