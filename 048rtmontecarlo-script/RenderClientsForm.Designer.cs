namespace Rendering
{
	partial class RenderClientsForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose ();
			}
			base.Dispose ( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.components = new System.ComponentModel.Container();
			this.clientsDataGrid = new System.Windows.Forms.DataGridView();
			this.renderClientsFormBindingSource = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)(this.clientsDataGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.renderClientsFormBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// clientsDataGrid
			// 
			this.clientsDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clientsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.clientsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.clientsDataGrid.Location = new System.Drawing.Point(12, 12);
			this.clientsDataGrid.Name = "clientsDataGrid";
			this.clientsDataGrid.Size = new System.Drawing.Size(351, 142);
			this.clientsDataGrid.TabIndex = 1;
			// 
			// renderClientsFormBindingSource
			// 
			this.renderClientsFormBindingSource.DataSource = typeof(Rendering.RenderClientsForm);
			// 
			// RenderClientsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(375, 166);
			this.Controls.Add(this.clientsDataGrid);
			this.Name = "RenderClientsForm";
			this.Text = "Render Client Management";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenderClientsForm_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.clientsDataGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.renderClientsFormBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView clientsDataGrid;
		private System.Windows.Forms.BindingSource renderClientsFormBindingSource;
	}
}