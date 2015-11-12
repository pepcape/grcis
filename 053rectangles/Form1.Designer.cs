namespace _053rectangles
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
      this.numericXres = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.labelImageName = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.numericYres = new System.Windows.Forms.NumericUpDown();
      this.buttonLoadImage = new System.Windows.Forms.Button();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericXres)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericYres)).BeginInit();
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
      // numericXres
      // 
      this.numericXres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericXres.Increment = new decimal( new int[] {
            50,
            0,
            0,
            0} );
      this.numericXres.Location = new System.Drawing.Point( 50, 406 );
      this.numericXres.Maximum = new decimal( new int[] {
            10000,
            0,
            0,
            0} );
      this.numericXres.Name = "numericXres";
      this.numericXres.Size = new System.Drawing.Size( 71, 20 );
      this.numericXres.TabIndex = 3;
      this.numericXres.Value = new decimal( new int[] {
            800,
            0,
            0,
            0} );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 14, 408 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 31, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Xres:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 142, 408 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 40, 13 );
      this.label2.TabIndex = 6;
      this.label2.Text = "Param:";
      // 
      // labelImageName
      // 
      this.labelImageName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelImageName.AutoSize = true;
      this.labelImageName.Location = new System.Drawing.Point( 252, 442 );
      this.labelImageName.Name = "labelImageName";
      this.labelImageName.Size = new System.Drawing.Size( 34, 13 );
      this.labelImageName.TabIndex = 8;
      this.labelImageName.Text = "Input:";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 14, 440 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 31, 13 );
      this.label3.TabIndex = 10;
      this.label3.Text = "Yres:";
      // 
      // numericYres
      // 
      this.numericYres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericYres.Increment = new decimal( new int[] {
            50,
            0,
            0,
            0} );
      this.numericYres.Location = new System.Drawing.Point( 50, 439 );
      this.numericYres.Maximum = new decimal( new int[] {
            10000,
            0,
            0,
            0} );
      this.numericYres.Name = "numericYres";
      this.numericYres.Size = new System.Drawing.Size( 71, 20 );
      this.numericYres.TabIndex = 11;
      this.numericYres.Value = new decimal( new int[] {
            600,
            0,
            0,
            0} );
      // 
      // buttonLoadImage
      // 
      this.buttonLoadImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoadImage.Location = new System.Drawing.Point( 145, 436 );
      this.buttonLoadImage.Name = "buttonLoadImage";
      this.buttonLoadImage.Size = new System.Drawing.Size( 90, 23 );
      this.buttonLoadImage.TabIndex = 12;
      this.buttonLoadImage.Text = "Input image";
      this.buttonLoadImage.UseVisualStyleBackColor = true;
      this.buttonLoadImage.Click += new System.EventHandler( this.buttonLoadImage_Click );
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRedraw.Location = new System.Drawing.Point( 379, 405 );
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size( 82, 23 );
      this.buttonRedraw.TabIndex = 13;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler( this.buttonRedraw_Click );
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Enabled = false;
      this.buttonSave.Location = new System.Drawing.Point( 601, 405 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 92, 23 );
      this.buttonSave.TabIndex = 14;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point( 189, 406 );
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size( 174, 20 );
      this.textParam.TabIndex = 15;
      this.textParam.Text = "1.00";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 474 );
      this.Controls.Add( this.textParam );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonRedraw );
      this.Controls.Add( this.buttonLoadImage );
      this.Controls.Add( this.numericYres );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.labelImageName );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericXres );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 650, 240 );
      this.Name = "Form1";
      this.Text = "053 rectangles";
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericXres)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericYres)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.NumericUpDown numericXres;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label labelImageName;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numericYres;
    private System.Windows.Forms.Button buttonLoadImage;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.TextBox textParam;
  }
}

