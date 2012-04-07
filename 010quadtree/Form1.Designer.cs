namespace _010quadtree
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
      this.numericXres = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonRecode = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.numericSeed = new System.Windows.Forms.NumericUpDown();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.labelResult = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.numericYres = new System.Windows.Forms.NumericUpDown();
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.buttonLoad = new System.Windows.Forms.Button();
      this.checkDiff = new System.Windows.Forms.CheckBox();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericXres)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).BeginInit();
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
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point( 585, 439 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 108, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // numericXres
      // 
      this.numericXres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            512,
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
      // buttonRecode
      // 
      this.buttonRecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRecode.Location = new System.Drawing.Point( 455, 405 );
      this.buttonRecode.Name = "buttonRecode";
      this.buttonRecode.Size = new System.Drawing.Size( 238, 23 );
      this.buttonRecode.TabIndex = 5;
      this.buttonRecode.Text = "Recode";
      this.buttonRecode.UseVisualStyleBackColor = true;
      this.buttonRecode.Click += new System.EventHandler( this.buttonRecode_Click );
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 142, 408 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 76, 13 );
      this.label2.TabIndex = 6;
      this.label2.Text = "Random seed:";
      // 
      // numericSeed
      // 
      this.numericSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSeed.Location = new System.Drawing.Point( 224, 406 );
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
      this.labelElapsed.Location = new System.Drawing.Point( 321, 444 );
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size( 48, 13 );
      this.labelElapsed.TabIndex = 8;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelResult
      // 
      this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelResult.AutoSize = true;
      this.labelResult.Location = new System.Drawing.Point( 423, 444 );
      this.labelResult.Name = "labelResult";
      this.labelResult.Size = new System.Drawing.Size( 32, 13 );
      this.labelResult.TabIndex = 9;
      this.labelResult.Text = "result";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 14, 444 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 31, 13 );
      this.label3.TabIndex = 10;
      this.label3.Text = "Yres:";
      // 
      // numericYres
      // 
      this.numericYres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            512,
            0,
            0,
            0} );
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point( 145, 438 );
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size( 150, 23 );
      this.buttonGenerate.TabIndex = 12;
      this.buttonGenerate.Text = "Generate";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler( this.buttonGenerate_Click );
      // 
      // buttonLoad
      // 
      this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoad.Location = new System.Drawing.Point( 324, 405 );
      this.buttonLoad.Name = "buttonLoad";
      this.buttonLoad.Size = new System.Drawing.Size( 110, 23 );
      this.buttonLoad.TabIndex = 13;
      this.buttonLoad.Text = "Load image";
      this.buttonLoad.UseVisualStyleBackColor = true;
      this.buttonLoad.Click += new System.EventHandler( this.buttonLoad_Click );
      // 
      // checkDiff
      // 
      this.checkDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkDiff.AutoSize = true;
      this.checkDiff.Checked = true;
      this.checkDiff.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkDiff.Location = new System.Drawing.Point( 504, 444 );
      this.checkDiff.Name = "checkDiff";
      this.checkDiff.Size = new System.Drawing.Size( 76, 17 );
      this.checkDiff.TabIndex = 14;
      this.checkDiff.Text = "Show diff?";
      this.checkDiff.UseVisualStyleBackColor = true;
      this.checkDiff.CheckedChanged += new System.EventHandler( this.checkDiff_CheckedChanged );
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 474 );
      this.Controls.Add( this.checkDiff );
      this.Controls.Add( this.buttonLoad );
      this.Controls.Add( this.buttonGenerate );
      this.Controls.Add( this.numericYres );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.labelResult );
      this.Controls.Add( this.labelElapsed );
      this.Controls.Add( this.numericSeed );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.buttonRecode );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericXres );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 720, 200 );
      this.Name = "Form1";
      this.Text = "010 quad-tree";
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericXres)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericYres)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown numericXres;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonRecode;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericSeed;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Label labelResult;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numericYres;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.Button buttonLoad;
    private System.Windows.Forms.CheckBox checkDiff;
  }
}

