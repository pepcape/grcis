using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modules
{
  public partial class FormHSV : Form
  {
    /// <summary>
    /// Associated raster module (to be notified under various conditions).
    /// </summary>
    protected IRasterModule module;

    /// <summary>
    /// If true, any of the values was changed and needs to by send to the module.
    /// </summary>
    protected bool dirty = true;

    public FormHSV (IRasterModule hModule)
    {
      module = hModule;

      InitializeComponent();
    }

    delegate void DataUpdateCallback (ModuleHSV m);

    /// <summary>
    /// Form elements must not be updated from foreign threads.
    /// Only the thread which has created this Form is capable to do it.
    /// In case of more complicated class dependence a data providing interface should be defined.
    /// Here we just use the 'ModuleHSV'.
    /// </summary>
    /// <param name="m">Module to get data from.</param>
    public void DataUpdate (ModuleHSV m)
    {
      if (numericHue.InvokeRequired)    // use any form element here...
      {
        DataUpdateCallback duc = new DataUpdateCallback(DataUpdate);
        BeginInvoke(duc, new object[] { m });
      }
      else
      {
        numericHue.Value      = Convert.ToDecimal(m.dH);
        textSaturation.Text   = string.Format(CultureInfo.InvariantCulture, "{0:g5}", m.mS);
        textValue.Text        = string.Format(CultureInfo.InvariantCulture, "{0:g5}", m.mV);
        textGamma.Text        = string.Format(CultureInfo.InvariantCulture, "{0:g5}", m.gamma);
        checkParallel.Checked = m.parallel;
        checkSlow.Checked     = m.slow;
      }
    }

    private void buttonRecompute_Click (object sender, EventArgs e)
    {
      if (module == null)
        return;

      if (dirty)
      {
        module.OnGuiWindowChanged();
        dirty = false;
      }

      module.UpdateRequest?.Invoke(module);
    }

    private void buttonReset_Click (object sender, EventArgs e)
    {
      numericHue.Value = Convert.ToDecimal(0);
      textSaturation.Text = "1.0";
      textValue.Text = "1.0";
      textGamma.Text = "1.0";
      checkParallel.Checked = true;
      checkSlow.Checked = false;

      module?.OnGuiWindowChanged();
      dirty = false;
    }

    private void buttonDeactivate_Click (object sender, EventArgs e)
    {
      if (module != null)
      {
        if (dirty)
          module.OnGuiWindowChanged();

        module.DeactivateRequest?.Invoke(module);
      }
    }

    private void FormHSV_FormClosed (object sender, FormClosedEventArgs e)
    {
      if (module != null)
      {
        if (dirty)
          module.OnGuiWindowChanged();

        module.OnGuiWindowClose();
      }
    }

    private void numericHue_ValueChanged (object sender, EventArgs e) => dirty = true;

    private void textSaturation_TextChanged (object sender, EventArgs e) => dirty = true;

    private void checkParallel_CheckedChanged (object sender, EventArgs e) => dirty = true;

    private void textValue_TextChanged (object sender, EventArgs e) => dirty = true;

    private void checkSlow_CheckedChanged (object sender, EventArgs e) => dirty = true;

    private void textGamma_TextChanged (object sender, EventArgs e) => dirty = true;
  }
}
