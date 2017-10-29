namespace _110animation
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
      this.buttonRenderAnim = new System.Windows.Forms.Button();
      this.buttonRender = new System.Windows.Forms.Button();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.numTime = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.numFrom = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.numTo = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.numFps = new System.Windows.Forms.NumericUpDown();
      this.buttonStop = new System.Windows.Forms.Button();
      this.buttonRes = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numTime)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numFrom)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numTo)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numFps)).BeginInit();
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
      this.panel1.Size = new System.Drawing.Size(680, 350);
      this.panel1.TabIndex = 0;
      //
      // pictureBox1
      //
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(680, 350);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      //
      // buttonRenderAnim
      //
      this.buttonRenderAnim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRenderAnim.Location = new System.Drawing.Point(554, 442);
      this.buttonRenderAnim.Name = "buttonRenderAnim";
      this.buttonRenderAnim.Size = new System.Drawing.Size(140, 23);
      this.buttonRenderAnim.TabIndex = 2;
      this.buttonRenderAnim.Text = "Render animation";
      this.buttonRenderAnim.UseVisualStyleBackColor = true;
      this.buttonRenderAnim.Click += new System.EventHandler(this.buttonRenderAnim_Click);
      //
      // buttonRender
      //
      this.buttonRender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRender.Location = new System.Drawing.Point(14, 441);
      this.buttonRender.Name = "buttonRender";
      this.buttonRender.Size = new System.Drawing.Size(97, 23);
      this.buttonRender.TabIndex = 5;
      this.buttonRender.Text = "Single image";
      this.buttonRender.UseVisualStyleBackColor = true;
      this.buttonRender.Click += new System.EventHandler(this.buttonRender_Click);
      //
      // labelElapsed
      //
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point(129, 447);
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size(48, 13);
      this.labelElapsed.TabIndex = 21;
      this.labelElapsed.Text = "Elapsed:";
      //
      // numTime
      //
      this.numTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numTime.DecimalPlaces = 3;
      this.numTime.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numTime.Location = new System.Drawing.Point(80, 378);
      this.numTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numTime.Name = "numTime";
      this.numTime.Size = new System.Drawing.Size(75, 20);
      this.numTime.TabIndex = 23;
      //
      // label1
      //
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 381);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(55, 13);
      this.label1.TabIndex = 24;
      this.label1.Text = "time (sec):";
      //
      // label2
      //
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(289, 381);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(59, 13);
      this.label2.TabIndex = 26;
      this.label2.Text = "From (sec):";
      //
      // numFrom
      //
      this.numFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.numFrom.DecimalPlaces = 3;
      this.numFrom.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numFrom.Location = new System.Drawing.Point(358, 378);
      this.numFrom.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numFrom.Name = "numFrom";
      this.numFrom.Size = new System.Drawing.Size(80, 20);
      this.numFrom.TabIndex = 25;
      //
      // label3
      //
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(447, 381);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(49, 13);
      this.label3.TabIndex = 28;
      this.label3.Text = "To (sec):";
      //
      // numTo
      //
      this.numTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.numTo.DecimalPlaces = 3;
      this.numTo.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numTo.Location = new System.Drawing.Point(505, 378);
      this.numTo.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numTo.Name = "numTo";
      this.numTo.Size = new System.Drawing.Size(79, 20);
      this.numTo.TabIndex = 27;
      this.numTo.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      //
      // label4
      //
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(599, 382);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(27, 13);
      this.label4.TabIndex = 30;
      this.label4.Text = "Fps:";
      //
      // numFps
      //
      this.numFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.numFps.DecimalPlaces = 1;
      this.numFps.Location = new System.Drawing.Point(635, 379);
      this.numFps.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.numFps.Name = "numFps";
      this.numFps.Size = new System.Drawing.Size(58, 20);
      this.numFps.TabIndex = 29;
      this.numFps.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
      //
      // buttonStop
      //
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point(454, 442);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(81, 23);
      this.buttonStop.TabIndex = 31;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      //
      // buttonRes
      //
      this.buttonRes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRes.Location = new System.Drawing.Point(175, 376);
      this.buttonRes.Name = "buttonRes";
      this.buttonRes.Size = new System.Drawing.Size(101, 23);
      this.buttonRes.TabIndex = 43;
      this.buttonRes.Text = "Resolution";
      this.buttonRes.UseVisualStyleBackColor = true;
      this.buttonRes.Click += new System.EventHandler(this.buttonRes_Click);
      //
      // textParam
      //
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(59, 410);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(634, 20);
      this.textParam.TabIndex = 45;
      this.textParam.Text = "1.0";
      //
      // label5
      //
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 412);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(40, 13);
      this.label5.TabIndex = 44;
      this.label5.Text = "Param:";
      //
      // Form1
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(712, 476);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.buttonRes);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.numFps);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.numTo);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.numFrom);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.numTime);
      this.Controls.Add(this.labelElapsed);
      this.Controls.Add(this.buttonRender);
      this.Controls.Add(this.buttonRenderAnim);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(720, 320);
      this.Name = "Form1";
      this.Text = "110 line animation";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numTime)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numFrom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numTo)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numFps)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonRenderAnim;
    private System.Windows.Forms.Button buttonRender;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.NumericUpDown numTime;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numFrom;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numTo;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown numFps;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.Button buttonRes;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label5;
  }
}

