using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace _034ascii
{
  public partial class Output : Form
  {
    public Output ()
    {
      InitializeComponent();
    }

    public string WndText
    {
      get;
      set;
    }

    public Font Fnt
    {
      get;
      set;
    }

    private void Output_Load ( object sender, EventArgs e )
    {
      txtOutput.Text = WndText;
      if ( Fnt != null )
        txtOutput.Font = Fnt;
      txtOutput.Select( 0, 0 );
    }

    private void btnSave_Click ( object sender, EventArgs e )
    {
      SaveFileDialog dlg = new SaveFileDialog();
      dlg.Title = "Save Text File";
      dlg.Filter = "Text Files (*.txt)|*.txt";

      if ( dlg.ShowDialog() == DialogResult.OK )
      {
        try
        {
          StreamWriter writer = new StreamWriter( dlg.FileName );

          writer.Write( WndText );
          writer.Close();
        }
        catch ( Exception exc )
        {
          MessageBox.Show( exc.Message );
        }
      }
    }
  }
}
