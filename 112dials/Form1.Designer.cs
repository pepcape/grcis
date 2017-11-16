namespace _112dials
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
      this.labelElapsed = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.numericYres = new System.Windows.Forms.NumericUpDown();
      this.buttonStop = new System.Windows.Forms.Button();
      this.buttonStart = new System.Windows.Forms.Button();
      this.buttonReset = new System.Windows.Forms.Button();
      this.checkAnim = new System.Windows.Forms.CheckBox();
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
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(13, 13);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(680, 380);
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(680, 380);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
      this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
      this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
      // 
      // numericXres
      // 
      this.numericXres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericXres.Location = new System.Drawing.Point(50, 406);
      this.numericXres.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numericXres.Name = "numericXres";
      this.numericXres.Size = new System.Drawing.Size(71, 20);
      this.numericXres.TabIndex = 3;
      this.numericXres.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(14, 408);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(31, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Xres:";
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point(389, 441);
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size(48, 13);
      this.labelElapsed.TabIndex = 8;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(14, 440);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(31, 13);
      this.label3.TabIndex = 10;
      this.label3.Text = "Yres:";
      // 
      // numericYres
      // 
      this.numericYres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericYres.Location = new System.Drawing.Point(50, 439);
      this.numericYres.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numericYres.Name = "numericYres";
      this.numericYres.Size = new System.Drawing.Size(71, 20);
      this.numericYres.TabIndex = 11;
      this.numericYres.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point(263, 436);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(110, 23);
      this.buttonStop.TabIndex = 12;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // buttonStart
      // 
      this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStart.Location = new System.Drawing.Point(263, 404);
      this.buttonStart.Name = "buttonStart";
      this.buttonStart.Size = new System.Drawing.Size(110, 23);
      this.buttonStart.TabIndex = 13;
      this.buttonStart.Text = "Start";
      this.buttonStart.UseVisualStyleBackColor = true;
      this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonReset.Location = new System.Drawing.Point(147, 404);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(93, 23);
      this.buttonReset.TabIndex = 14;
      this.buttonReset.Text = "Reset";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      // 
      // checkAnim
      // 
      this.checkAnim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAnim.AutoSize = true;
      this.checkAnim.Location = new System.Drawing.Point(151, 440);
      this.checkAnim.Name = "checkAnim";
      this.checkAnim.Size = new System.Drawing.Size(89, 17);
      this.checkAnim.TabIndex = 15;
      this.checkAnim.Text = "save frames?";
      this.checkAnim.UseVisualStyleBackColor = true;
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(392, 407);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(301, 20);
      this.textParam.TabIndex = 16;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(712, 474);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.checkAnim);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.buttonStart);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.numericYres);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.labelElapsed);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.numericXres);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(720, 200);
      this.Name = "Form1";
      this.Text = "112 dials";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericXres)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericYres)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.NumericUpDown numericXres;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numericYres;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.Button buttonStart;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.CheckBox checkAnim;
    private System.Windows.Forms.TextBox textParam;
  }
}

