using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _058marbles
{
  static class Program
  {
    static public Form form = null;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main ()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault( false );
      Application.Run( form = new Form1() );
    }
  }
}
