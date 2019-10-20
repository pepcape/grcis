using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modules
{
  public partial class FormHSV : Form
  {
    /// <summary>
    /// Associated raster module (to be notified in case of form close).
    /// </summary>
    protected IRasterModule module;

    public FormHSV (IRasterModule hModule)
    {
      module = hModule;

      InitializeComponent();
    }

    private void buttonRecompute_Click (object sender, EventArgs e)
    {

    }

    private void buttonReset_Click (object sender, EventArgs e)
    {

    }

    private void buttonDeactivate_Click (object sender, EventArgs e)
    {

    }
  }
}
