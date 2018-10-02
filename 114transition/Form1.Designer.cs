namespace _114transition
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
      this.buttonOpen1 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonOpen2 = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.buttonStop = new System.Windows.Forms.Button();
      this.trackBarTime = new System.Windows.Forms.TrackBar();
      this.labelMin = new System.Windows.Forms.Label();
      this.labelMax = new System.Windows.Forms.Label();
      this.labelStatus = new System.Windows.Forms.Label();
      this.labelTimeValue = new System.Windows.Forms.Label();
      this.buttonRun = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).BeginInit();
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
      this.panel1.Size = new System.Drawing.Size(683, 380);
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(683, 380);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
      this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
      // 
      // buttonOpen1
      // 
      this.buttonOpen1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen1.Location = new System.Drawing.Point(13, 403);
      this.buttonOpen1.Name = "buttonOpen1";
      this.buttonOpen1.Size = new System.Drawing.Size(85, 23);
      this.buttonOpen1.TabIndex = 1;
      this.buttonOpen1.Text = "Image 1";
      this.buttonOpen1.UseVisualStyleBackColor = true;
      this.buttonOpen1.Click += new System.EventHandler(this.buttonOpen1_Click);
      this.buttonOpen1.MouseHover += new System.EventHandler(this.buttonOpen1_MouseHover);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(107, 408);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(40, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Param:";
      // 
      // buttonOpen2
      // 
      this.buttonOpen2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen2.Location = new System.Drawing.Point(13, 435);
      this.buttonOpen2.Name = "buttonOpen2";
      this.buttonOpen2.Size = new System.Drawing.Size(85, 23);
      this.buttonOpen2.TabIndex = 4;
      this.buttonOpen2.Text = "Image 2";
      this.buttonOpen2.UseVisualStyleBackColor = true;
      this.buttonOpen2.Click += new System.EventHandler(this.buttonOpen2_Click);
      this.buttonOpen2.MouseHover += new System.EventHandler(this.buttonOpen2_MouseHover);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point(150, 405);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(235, 20);
      this.textParam.TabIndex = 3;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      this.textParam.MouseHover += new System.EventHandler(this.textParam_MouseHover);
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point(247, 436);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(44, 23);
      this.buttonStop.TabIndex = 7;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // trackBarTime
      // 
      this.trackBarTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.trackBarTime.LargeChange = 20;
      this.trackBarTime.Location = new System.Drawing.Point(399, 408);
      this.trackBarTime.Maximum = 400;
      this.trackBarTime.Name = "trackBarTime";
      this.trackBarTime.Size = new System.Drawing.Size(299, 45);
      this.trackBarTime.SmallChange = 0;
      this.trackBarTime.TabIndex = 9;
      this.trackBarTime.TickFrequency = 5;
      this.trackBarTime.TickStyle = System.Windows.Forms.TickStyle.None;
      this.trackBarTime.Value = 200;
      this.trackBarTime.ValueChanged += new System.EventHandler(this.trackBarTime_ValueChanged);
      // 
      // labelMin
      // 
      this.labelMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelMin.AutoSize = true;
      this.labelMin.Location = new System.Drawing.Point(405, 438);
      this.labelMin.Name = "labelMin";
      this.labelMin.Size = new System.Drawing.Size(28, 13);
      this.labelMin.TabIndex = 10;
      this.labelMin.Text = "0.00";
      // 
      // labelMax
      // 
      this.labelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelMax.Location = new System.Drawing.Point(642, 438);
      this.labelMax.Name = "labelMax";
      this.labelMax.Size = new System.Drawing.Size(50, 13);
      this.labelMax.TabIndex = 12;
      this.labelMax.Text = "1.00";
      this.labelMax.TextAlign = System.Drawing.ContentAlignment.BottomRight;
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(108, 440);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(53, 13);
      this.labelStatus.TabIndex = 5;
      this.labelStatus.Text = "-- status --";
      this.labelStatus.MouseHover += new System.EventHandler(this.labelStatus_MouseHover);
      // 
      // labelTimeValue
      // 
      this.labelTimeValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelTimeValue.AutoSize = true;
      this.labelTimeValue.Location = new System.Drawing.Point(538, 439);
      this.labelTimeValue.Name = "labelTimeValue";
      this.labelTimeValue.Size = new System.Drawing.Size(28, 13);
      this.labelTimeValue.TabIndex = 11;
      this.labelTimeValue.Text = "0.50";
      this.labelTimeValue.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      // 
      // buttonRun
      // 
      this.buttonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRun.Enabled = false;
      this.buttonRun.Location = new System.Drawing.Point(195, 436);
      this.buttonRun.Name = "buttonRun";
      this.buttonRun.Size = new System.Drawing.Size(45, 23);
      this.buttonRun.TabIndex = 6;
      this.buttonRun.Text = "Run";
      this.buttonRun.UseVisualStyleBackColor = true;
      this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonSave.Enabled = false;
      this.buttonSave.Location = new System.Drawing.Point(298, 436);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(87, 23);
      this.buttonSave.TabIndex = 8;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(710, 466);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.buttonRun);
      this.Controls.Add(this.labelTimeValue);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.labelMax);
      this.Controls.Add(this.labelMin);
      this.Controls.Add(this.trackBarTime);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.buttonOpen2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.buttonOpen1);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(600, 300);
      this.Name = "Form1";
      this.Text = "114 image transition";
      this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonOpen1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonOpen2;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.TrackBar trackBarTime;
    private System.Windows.Forms.Label labelMin;
    private System.Windows.Forms.Label labelMax;
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.Label labelTimeValue;
    private System.Windows.Forms.Button buttonRun;
    private System.Windows.Forms.Button buttonSave;
  }
}

