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
	public partial class RenderClientsForm : Form
	{
	  public static RenderClientsForm instance; //singleton

		public BindingList<Client> clients;

		public RenderClientsForm ()
		{
			InitializeComponent ();

		  clients = new BindingList<Client>();

		  clients.Add ( new Client { Name = "test1", Address = "1.1.1.1" } );
		  clients.Add ( new Client { Name = "test2", Address = "1.1.1.2" } );
		  clients.Add ( new Client { Name = "test3", Address = "1.1.1.3" } );

		  clientsDataGrid.AutoGenerateColumns = false;

			clientsDataGrid.DataSource = clients;

		  DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
		  column1.Name = "Client Name";
		  column1.DataPropertyName = "Name";
			clientsDataGrid.Columns.Add ( column1 );

			DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
		  column2.Name = "IP Address";
		  column2.DataPropertyName = "address";
			clientsDataGrid.Columns.Add ( column2 );

		  clientsDataGrid.Update ();

			instance = this;
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


	  public static void ParseAddress ()
	  {
	    bool isValidIP = false;

	    do
	    {
	      string line = Console.ReadLine ();

	      if ( line != null )
	      {
	        //isValidIP = IPAddress.TryParse ( line, out ipAdr );
	      }

	      if ( !isValidIP )
	      {
	        Console.WriteLine ( "IP is not in valid format!" );
	        Console.WriteLine ( "Try to enter it again:" );
	      }

	    } while ( !isValidIP );
	  }
	}


  public class Client
  {
		private string name;
		private IPAddress address;

		public string Name { get => name; set => name = value; }
		public string Address { get => GetIPAddress (); set => CheckAndSetIPAddress ( value );
		}

    private void CheckAndSetIPAddress ( string value )
    {
      bool isValidIP = IPAddress.TryParse ( value, out address );

      if ( !isValidIP )
      {
				address = null;
      }
		}

    private string GetIPAddress ()
    {
      if ( address == null )
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
