using System.Windows.Forms;

namespace _066histogram
{
  public partial class HistogramForm : Form
  {
    protected Form1 parent;

    public HistogramForm ( Form1 par )
    {
      parent = par;
      InitializeComponent();
    }

    private void HistogramForm_FormClosed ( object sender, FormClosedEventArgs e )
    {
      parent.histogramForm = null;
    }
  }
}
