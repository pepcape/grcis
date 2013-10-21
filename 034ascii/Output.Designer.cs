namespace _034ascii
{
  partial class Output
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
      if ( disposing && (components != null) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.txtOutput = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtOutput
      // 
      this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtOutput.Font = new System.Drawing.Font( "Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
      this.txtOutput.Location = new System.Drawing.Point( 0, 0 );
      this.txtOutput.Multiline = true;
      this.txtOutput.Name = "txtOutput";
      this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtOutput.Size = new System.Drawing.Size( 815, 405 );
      this.txtOutput.TabIndex = 0;
      this.txtOutput.WordWrap = false;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Location = new System.Drawing.Point( 673, 414 );
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size( 127, 26 );
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save text file";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler( this.btnSave_Click );
      // 
      // Output
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 814, 452 );
      this.Controls.Add( this.btnSave );
      this.Controls.Add( this.txtOutput );
      this.MinimumSize = new System.Drawing.Size( 230, 180 );
      this.Name = "Output";
      this.Text = "ASCII art output";
      this.Load += new System.EventHandler( this.Output_Load );
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtOutput;
    private System.Windows.Forms.Button btnSave;
  }
}