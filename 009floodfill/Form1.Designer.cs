namespace _009floodfill
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
      this.numericSize = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.numericSeed = new System.Windows.Forms.NumericUpDown();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.labelHash = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSize)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).BeginInit();
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
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point( 585, 411 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 108, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // numericSize
      // 
      this.numericSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSize.Location = new System.Drawing.Point( 50, 414 );
      this.numericSize.Maximum = new decimal( new int[] {
            1000000,
            0,
            0,
            0} );
      this.numericSize.Name = "numericSize";
      this.numericSize.Size = new System.Drawing.Size( 71, 20 );
      this.numericSize.TabIndex = 3;
      this.numericSize.Value = new decimal( new int[] {
            30,
            0,
            0,
            0} );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 14, 416 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 27, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Fills:";
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRedraw.Location = new System.Drawing.Point( 324, 411 );
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size( 110, 23 );
      this.buttonRedraw.TabIndex = 5;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler( this.buttonRedraw_Click );
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 142, 416 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 76, 13 );
      this.label2.TabIndex = 6;
      this.label2.Text = "Random seed:";
      // 
      // numericSeed
      // 
      this.numericSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSeed.Location = new System.Drawing.Point( 224, 414 );
      this.numericSeed.Maximum = new decimal( new int[] {
            1000000000,
            0,
            0,
            0} );
      this.numericSeed.Name = "numericSeed";
      this.numericSeed.Size = new System.Drawing.Size( 71, 20 );
      this.numericSeed.TabIndex = 7;
      this.numericSeed.Value = new decimal( new int[] {
            12,
            0,
            0,
            0} );
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point( 452, 421 );
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size( 48, 13 );
      this.labelElapsed.TabIndex = 8;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelHash
      // 
      this.labelHash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelHash.AutoSize = true;
      this.labelHash.Location = new System.Drawing.Point( 452, 402 );
      this.labelHash.Name = "labelHash";
      this.labelHash.Size = new System.Drawing.Size( 30, 13 );
      this.labelHash.TabIndex = 9;
      this.labelHash.Text = "hash";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.labelHash );
      this.Controls.Add( this.labelElapsed );
      this.Controls.Add( this.numericSeed );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.buttonRedraw );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericSize );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 720, 200 );
      this.Name = "Form1";
      this.Text = "009 flood-fill";
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSize)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown numericSize;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericSeed;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Label labelHash;
  }
}

