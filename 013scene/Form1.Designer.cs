namespace _013scene
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
      this.buttonOpen = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.numericAzimuth = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.labelFaces = new System.Windows.Forms.Label();
      this.checkNormals = new System.Windows.Forms.CheckBox();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).BeginInit();
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
      this.panel1.Size = new System.Drawing.Size( 680, 380 );
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point( 0, 0 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 680, 380 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point( 13, 411 );
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size( 108, 23 );
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load scene";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler( this.buttonOpen_Click );
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
      // numericAzimuth
      // 
      this.numericAzimuth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericAzimuth.DecimalPlaces = 1;
      this.numericAzimuth.Location = new System.Drawing.Point( 268, 414 );
      this.numericAzimuth.Maximum = new decimal( new int[] {
            360,
            0,
            0,
            0} );
      this.numericAzimuth.Name = "numericAzimuth";
      this.numericAzimuth.Size = new System.Drawing.Size( 62, 20 );
      this.numericAzimuth.TabIndex = 3;
      this.numericAzimuth.Value = new decimal( new int[] {
            20,
            0,
            0,
            0} );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 218, 417 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 47, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Azimuth:";
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRedraw.Location = new System.Drawing.Point( 417, 412 );
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size( 95, 23 );
      this.buttonRedraw.TabIndex = 5;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler( this.buttonRedraw_Click );
      // 
      // labelFaces
      // 
      this.labelFaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelFaces.AutoSize = true;
      this.labelFaces.Location = new System.Drawing.Point( 133, 417 );
      this.labelFaces.Name = "labelFaces";
      this.labelFaces.Size = new System.Drawing.Size( 33, 13 );
      this.labelFaces.TabIndex = 6;
      this.labelFaces.Text = "faces";
      // 
      // checkNormals
      // 
      this.checkNormals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkNormals.AutoSize = true;
      this.checkNormals.Location = new System.Drawing.Point( 346, 417 );
      this.checkNormals.Name = "checkNormals";
      this.checkNormals.Size = new System.Drawing.Size( 62, 17 );
      this.checkNormals.TabIndex = 7;
      this.checkNormals.Text = "normals";
      this.checkNormals.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.checkNormals );
      this.Controls.Add( this.labelFaces );
      this.Controls.Add( this.buttonRedraw );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericAzimuth );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonOpen );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 660, 200 );
      this.Name = "Form1";
      this.Text = "013 scene";
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown numericAzimuth;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.Label labelFaces;
    private System.Windows.Forms.CheckBox checkNormals;
  }
}

