namespace _017graph
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
      this.numericAzimuth = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.numericElevation = new System.Windows.Forms.NumericUpDown();
      this.numericParam = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.checkPerspective = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.numericVariant = new System.Windows.Forms.NumericUpDown();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.numericRows = new System.Windows.Forms.NumericUpDown();
      this.numericColumns = new System.Windows.Forms.NumericUpDown();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericElevation)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericVariant)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericRows)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericColumns)).BeginInit();
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
      this.numericAzimuth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.numericAzimuth.DecimalPlaces = 1;
      this.numericAzimuth.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.numericAzimuth.Location = new System.Drawing.Point( 368, 378 );
      this.numericAzimuth.Maximum = new decimal( new int[] {
            720,
            0,
            0,
            0} );
      this.numericAzimuth.Minimum = new decimal( new int[] {
            720,
            0,
            0,
            -2147483648} );
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
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 306, 418 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 54, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Elevation:";
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRedraw.Location = new System.Drawing.Point( 446, 412 );
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size( 125, 23 );
      this.buttonRedraw.TabIndex = 5;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler( this.buttonRedraw_Click );
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 306, 380 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 47, 13 );
      this.label2.TabIndex = 9;
      this.label2.Text = "Azimuth:";
      // 
      // numericElevation
      // 
      this.numericElevation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.numericElevation.DecimalPlaces = 1;
      this.numericElevation.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.numericElevation.Location = new System.Drawing.Point( 368, 414 );
      this.numericElevation.Maximum = new decimal( new int[] {
            89,
            0,
            0,
            0} );
      this.numericElevation.Minimum = new decimal( new int[] {
            89,
            0,
            0,
            -2147483648} );
      this.numericElevation.Name = "numericElevation";
      this.numericElevation.Size = new System.Drawing.Size( 62, 20 );
      this.numericElevation.TabIndex = 10;
      this.numericElevation.Value = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      // 
      // numericParam
      // 
      this.numericParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericParam.DecimalPlaces = 2;
      this.numericParam.Increment = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.numericParam.Location = new System.Drawing.Point( 82, 412 );
      this.numericParam.Name = "numericParam";
      this.numericParam.Size = new System.Drawing.Size( 62, 20 );
      this.numericParam.TabIndex = 12;
      this.numericParam.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 13, 415 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 46, 13 );
      this.label3.TabIndex = 13;
      this.label3.Text = "Domain:";
      // 
      // checkPerspective
      // 
      this.checkPerspective.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkPerspective.AutoSize = true;
      this.checkPerspective.Checked = true;
      this.checkPerspective.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkPerspective.Location = new System.Drawing.Point( 450, 380 );
      this.checkPerspective.Name = "checkPerspective";
      this.checkPerspective.Size = new System.Drawing.Size( 55, 17 );
      this.checkPerspective.TabIndex = 14;
      this.checkPerspective.Text = "persp.";
      this.checkPerspective.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point( 16, 381 );
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size( 43, 13 );
      this.label4.TabIndex = 15;
      this.label4.Text = "Variant:";
      // 
      // numericVariant
      // 
      this.numericVariant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericVariant.Location = new System.Drawing.Point( 82, 378 );
      this.numericVariant.Name = "numericVariant";
      this.numericVariant.Size = new System.Drawing.Size( 62, 20 );
      this.numericVariant.TabIndex = 16;
      // 
      // label5
      // 
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point( 169, 383 );
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size( 37, 13 );
      this.label5.TabIndex = 17;
      this.label5.Text = "Rows:";
      // 
      // label6
      // 
      this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point( 168, 417 );
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size( 50, 13 );
      this.label6.TabIndex = 18;
      this.label6.Text = "Columns:";
      // 
      // numericRows
      // 
      this.numericRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericRows.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.numericRows.Location = new System.Drawing.Point( 234, 379 );
      this.numericRows.Maximum = new decimal( new int[] {
            300,
            0,
            0,
            0} );
      this.numericRows.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.numericRows.Name = "numericRows";
      this.numericRows.Size = new System.Drawing.Size( 49, 20 );
      this.numericRows.TabIndex = 19;
      this.numericRows.Value = new decimal( new int[] {
            20,
            0,
            0,
            0} );
      // 
      // numericColumns
      // 
      this.numericColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericColumns.Increment = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.numericColumns.Location = new System.Drawing.Point( 234, 413 );
      this.numericColumns.Maximum = new decimal( new int[] {
            300,
            0,
            0,
            0} );
      this.numericColumns.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.numericColumns.Name = "numericColumns";
      this.numericColumns.Size = new System.Drawing.Size( 49, 20 );
      this.numericColumns.TabIndex = 20;
      this.numericColumns.Value = new decimal( new int[] {
            20,
            0,
            0,
            0} );
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point( 524, 382 );
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size( 48, 13 );
      this.labelElapsed.TabIndex = 21;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.labelElapsed );
      this.Controls.Add( this.numericColumns );
      this.Controls.Add( this.numericRows );
      this.Controls.Add( this.label6 );
      this.Controls.Add( this.label5 );
      this.Controls.Add( this.numericVariant );
      this.Controls.Add( this.label4 );
      this.Controls.Add( this.checkPerspective );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.numericParam );
      this.Controls.Add( this.numericElevation );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.buttonRedraw );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericAzimuth );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 660, 200 );
      this.Name = "Form1";
      this.Text = "017 graph";
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericElevation)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericVariant)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericRows)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericColumns)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown numericAzimuth;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericElevation;
    private System.Windows.Forms.NumericUpDown numericParam;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox checkPerspective;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown numericVariant;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown numericRows;
    private System.Windows.Forms.NumericUpDown numericColumns;
    private System.Windows.Forms.Label labelElapsed;
  }
}

