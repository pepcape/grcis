namespace _111tree
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
      this.numericSize = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonQuery = new System.Windows.Forms.Button();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label2 = new System.Windows.Forms.Label();
      this.numericQuery = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.numericSeed = new System.Windows.Forms.NumericUpDown();
      this.checkVisual = new System.Windows.Forms.CheckBox();
      this.labelHash = new System.Windows.Forms.Label();
      this.labelStat = new System.Windows.Forms.Label();
      this.numericK = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.numericSize)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericQuery)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericK)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point(185, 659);
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size(77, 23);
      this.buttonGenerate.TabIndex = 1;
      this.buttonGenerate.Text = "Build";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(615, 660);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(78, 25);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // numericSize
      // 
      this.numericSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSize.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.numericSize.Location = new System.Drawing.Point(72, 662);
      this.numericSize.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
      this.numericSize.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.numericSize.Name = "numericSize";
      this.numericSize.Size = new System.Drawing.Size(96, 20);
      this.numericSize.TabIndex = 3;
      this.numericSize.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 664);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(54, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Data size:";
      // 
      // buttonQuery
      // 
      this.buttonQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonQuery.Location = new System.Drawing.Point(523, 661);
      this.buttonQuery.Name = "buttonQuery";
      this.buttonQuery.Size = new System.Drawing.Size(72, 23);
      this.buttonQuery.TabIndex = 5;
      this.buttonQuery.Text = "Query";
      this.buttonQuery.UseVisualStyleBackColor = true;
      this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Location = new System.Drawing.Point(13, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(680, 601);
      this.pictureBox1.TabIndex = 6;
      this.pictureBox1.TabStop = false;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(284, 664);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(59, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "Query size:";
      // 
      // numericQuery
      // 
      this.numericQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericQuery.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
      this.numericQuery.Location = new System.Drawing.Point(346, 662);
      this.numericQuery.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
      this.numericQuery.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericQuery.Name = "numericQuery";
      this.numericQuery.Size = new System.Drawing.Size(67, 20);
      this.numericQuery.TabIndex = 8;
      this.numericQuery.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(13, 630);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(76, 13);
      this.label3.TabIndex = 9;
      this.label3.Text = "Random seed:";
      // 
      // numericSeed
      // 
      this.numericSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSeed.Location = new System.Drawing.Point(99, 627);
      this.numericSeed.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
      this.numericSeed.Name = "numericSeed";
      this.numericSeed.Size = new System.Drawing.Size(130, 20);
      this.numericSeed.TabIndex = 10;
      // 
      // checkVisual
      // 
      this.checkVisual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVisual.AutoSize = true;
      this.checkVisual.Checked = true;
      this.checkVisual.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVisual.Location = new System.Drawing.Point(257, 630);
      this.checkVisual.Name = "checkVisual";
      this.checkVisual.Size = new System.Drawing.Size(63, 17);
      this.checkVisual.TabIndex = 11;
      this.checkVisual.Text = " Visual?";
      this.checkVisual.UseVisualStyleBackColor = true;
      // 
      // labelHash
      // 
      this.labelHash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelHash.AutoSize = true;
      this.labelHash.Location = new System.Drawing.Point(331, 632);
      this.labelHash.Name = "labelHash";
      this.labelHash.Size = new System.Drawing.Size(30, 13);
      this.labelHash.TabIndex = 12;
      this.labelHash.Text = "hash";
      // 
      // labelStat
      // 
      this.labelStat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelStat.AutoSize = true;
      this.labelStat.Location = new System.Drawing.Point(446, 632);
      this.labelStat.Name = "labelStat";
      this.labelStat.Size = new System.Drawing.Size(48, 13);
      this.labelStat.TabIndex = 13;
      this.labelStat.Text = "Elapsed:";
      // 
      // numericK
      // 
      this.numericK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericK.Location = new System.Drawing.Point(445, 662);
      this.numericK.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numericK.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericK.Name = "numericK";
      this.numericK.Size = new System.Drawing.Size(61, 20);
      this.numericK.TabIndex = 15;
      this.numericK.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(423, 664);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(17, 13);
      this.label4.TabIndex = 14;
      this.label4.Text = "K:";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(712, 694);
      this.Controls.Add(this.numericK);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.labelStat);
      this.Controls.Add(this.labelHash);
      this.Controls.Add(this.checkVisual);
      this.Controls.Add(this.numericSeed);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.numericQuery);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.buttonQuery);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.numericSize);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.buttonGenerate);
      this.MinimumSize = new System.Drawing.Size(728, 400);
      this.Name = "Form1";
      this.Text = "111 Spatial tree";
      ((System.ComponentModel.ISupportInitialize)(this.numericSize)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericQuery)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericK)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown numericSize;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonQuery;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericQuery;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numericSeed;
    private System.Windows.Forms.CheckBox checkVisual;
    private System.Windows.Forms.Label labelHash;
    private System.Windows.Forms.Label labelStat;
    private System.Windows.Forms.NumericUpDown numericK;
    private System.Windows.Forms.Label label4;
  }
}

