using System.ComponentModel;
using System.Windows.Forms;

namespace Rendering
{
  public partial class RenderClientsForm: Form
  {
    public static RenderClientsForm instance; //singleton

    public BindingList<Client> clients;

    /// <summary>
    /// Form constructor
    /// Creates BindingList and binds it to the correct columns of clientsDataGrid
    /// </summary>
    public RenderClientsForm ()
    {
      InitializeComponent ();

      instance = this;

      clients = new BindingList<Client> ();

      clientsDataGrid.AutoGenerateColumns = false;

      clientsDataGrid.DataSource = clients;

      DataGridViewColumn column1 = new DataGridViewTextBoxColumn ();
      column1.Name             = "Client Name";
      column1.DataPropertyName = "Name";
      clientsDataGrid.Columns.Add(column1);

      DataGridViewColumn column2 = new DataGridViewTextBoxColumn ();
      column2.Name             = "IP Address";
      column2.DataPropertyName = "AddressString";
      clientsDataGrid.Columns.Add(column2);

      clientsDataGrid.Update ();
    }

    /// <summary>
    /// Hides form instead of Closing it (only in case of closing by user)
    /// </summary>
    private void RenderClientsForm_FormClosing (object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        Hide ();
      }
    }
  }
}
