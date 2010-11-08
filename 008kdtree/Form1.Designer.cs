namespace _008kdtree
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
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.numericParam = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonQuery = new System.Windows.Forms.Button();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label2 = new System.Windows.Forms.Label();
      this.numericQuery = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericQuery)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point( 184, 411 );
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size( 100, 23 );
      this.buttonGenerate.TabIndex = 1;
      this.buttonGenerate.Text = "Build data";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler( this.buttonGenerate_Click );
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point( 591, 411 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 102, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // numericParam
      // 
      this.numericParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericParam.Increment = new decimal( new int[] {
            1000,
            0,
            0,
            0} );
      this.numericParam.Location = new System.Drawing.Point( 72, 414 );
      this.numericParam.Maximum = new decimal( new int[] {
            10000000,
            0,
            0,
            0} );
      this.numericParam.Name = "numericParam";
      this.numericParam.Size = new System.Drawing.Size( 96, 20 );
      this.numericParam.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 12, 416 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 54, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Data size:";
      // 
      // buttonQuery
      // 
      this.buttonQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonQuery.Location = new System.Drawing.Point( 477, 411 );
      this.buttonQuery.Name = "buttonQuery";
      this.buttonQuery.Size = new System.Drawing.Size( 89, 23 );
      this.buttonQuery.TabIndex = 5;
      this.buttonQuery.Text = "Start query";
      this.buttonQuery.UseVisualStyleBackColor = true;
      this.buttonQuery.Click += new System.EventHandler( this.buttonQuery_Click );
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Location = new System.Drawing.Point( 13, 12 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 680, 380 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 6;
      this.pictureBox1.TabStop = false;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 304, 416 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 59, 13 );
      this.label2.TabIndex = 7;
      this.label2.Text = "Query size:";
      // 
      // numericQuery
      // 
      this.numericQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericQuery.Increment = new decimal( new int[] {
            100,
            0,
            0,
            0} );
      this.numericQuery.Location = new System.Drawing.Point( 369, 414 );
      this.numericQuery.Maximum = new decimal( new int[] {
            1000000,
            0,
            0,
            0} );
      this.numericQuery.Name = "numericQuery";
      this.numericQuery.Size = new System.Drawing.Size( 82, 20 );
      this.numericQuery.TabIndex = 8;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.numericQuery );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.pictureBox1 );
      this.Controls.Add( this.buttonQuery );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericParam );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonGenerate );
      this.MinimumSize = new System.Drawing.Size( 620, 200 );
      this.Name = "Form1";
      this.Text = "008 KD-tree";
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericQuery)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown numericParam;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonQuery;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericQuery;
  }
}

