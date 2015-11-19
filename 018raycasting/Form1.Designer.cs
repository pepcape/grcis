namespace _018raycasting
{
  partial class Form1
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
      this.panel1 = new System.Windows.Forms.Panel();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.buttonSave = new System.Windows.Forms.Button();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.labelSample = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.AutoScroll = true;
      this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add( this.pictureBox1 );
      this.panel1.Location = new System.Drawing.Point( 13, 13 );
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size( 680, 350 );
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point( 0, 0 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 680, 350 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler( this.pictureBox1_MouseDown );
      this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler( this.pictureBox1_MouseMove );
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point( 586, 412 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 107, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRedraw.Location = new System.Drawing.Point( 13, 411 );
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size( 125, 23 );
      this.buttonRedraw.TabIndex = 5;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler( this.buttonRedraw_Click );
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point( 161, 417 );
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size( 48, 13 );
      this.labelElapsed.TabIndex = 21;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelSample
      // 
      this.labelSample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelSample.AutoSize = true;
      this.labelSample.Location = new System.Drawing.Point( 266, 417 );
      this.labelSample.Name = "labelSample";
      this.labelSample.Size = new System.Drawing.Size( 45, 13 );
      this.labelSample.TabIndex = 22;
      this.labelSample.Text = "Sample:";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.labelSample );
      this.Controls.Add( this.labelElapsed );
      this.Controls.Add( this.buttonRedraw );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 690, 200 );
      this.Name = "Form1";
      this.Text = "018 ray-casting";
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Label labelSample;
  }
}

