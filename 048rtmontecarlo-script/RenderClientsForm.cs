using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _048rtmontecarlo
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
      clientsDataGrid.Columns.Add ( column1 );

      DataGridViewColumn column2 = new DataGridViewTextBoxColumn ();
      column2.Name             = "IP Address";
      column2.DataPropertyName = "AddressString";
      clientsDataGrid.Columns.Add ( column2 );

      clientsDataGrid.Update ();
    }

    /// <summary>
    /// Hides form instead of Closing it (only in case of closing by user)
    /// </summary>
    private void RenderClientsForm_FormClosing ( object sender, FormClosingEventArgs e )
    {
      if ( e.CloseReason == CloseReason.UserClosing )
      {
        e.Cancel = true;
        Hide ();
      }
    }
  }


  /// <summary>
  /// Represents one render client - name and IPaddress got from clientsDataGrid
  /// </summary>
  public class Client
  {
    private string    name;
    public  IPAddress address;

    public string Name
    {
      get => name;
      set => name = value;
    }

    public string AddressString
    {
      get => GetIPAddress ();
      set => CheckAndSetIPAddress ( value );
    } // string representation for the need of text in clientsDataGrid

    private void CheckAndSetIPAddress ( string value )
    {
      bool isValidIP = IPAddress.TryParse ( value, out address );

      if ( !isValidIP )
      {
        address = IPAddress.Parse ( "0.0.0.0" );
        ;
      }
    }

    private string GetIPAddress ()
    {
      if ( address == null )
      {
        return "";
      }

      if ( address.ToString () == "0.0.0.0" )
      {
        return "Invalid IP Address!";
      }
      else
      {
        return address.ToString ();
      }
    }
  }
}