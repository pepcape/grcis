namespace _066histogram
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
      this.label1 = new System.Windows.Forms.Label();
      this.buttonHistogram = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.buttonDPCM = new System.Windows.Forms.Button();
      this.buttonPCM = new System.Windows.Forms.Button();
      this.labelStatus = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(13, 436);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(86, 23);
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load image";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(111, 441);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(40, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Param:";
      // 
      // buttonHistogram
      // 
      this.buttonHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonHistogram.Location = new System.Drawing.Point(396, 436);
      this.buttonHistogram.Name = "buttonHistogram";
      this.buttonHistogram.Size = new System.Drawing.Size(77, 23);
      this.buttonHistogram.TabIndex = 6;
      this.buttonHistogram.Text = "Histogram";
      this.buttonHistogram.UseVisualStyleBackColor = true;
      this.buttonHistogram.Click += new System.EventHandler(this.buttonHistogram_Click);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(156, 438);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(224, 20);
      this.textParam.TabIndex = 7;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      // 
      // buttonDPCM
      // 
      this.buttonDPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonDPCM.Location = new System.Drawing.Point(549, 436);
      this.buttonDPCM.Name = "buttonDPCM";
      this.buttonDPCM.Size = new System.Drawing.Size(59, 23);
      this.buttonDPCM.TabIndex = 8;
      this.buttonDPCM.Text = "DPCM";
      this.buttonDPCM.UseVisualStyleBackColor = true;
      this.buttonDPCM.Click += new System.EventHandler(this.buttonDPCM_Click);
      // 
      // buttonPCM
      // 
      this.buttonPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonPCM.Location = new System.Drawing.Point(482, 436);
      this.buttonPCM.Name = "buttonPCM";
      this.buttonPCM.Size = new System.Drawing.Size(59, 23);
      this.buttonPCM.TabIndex = 9;
      this.buttonPCM.Text = "PCM";
      this.buttonPCM.UseVisualStyleBackColor = true;
      this.buttonPCM.Click += new System.EventHandler(this.buttonPCM_Click);
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(14, 410);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(43, 13);
      this.labelStatus.TabIndex = 10;
      this.labelStatus.Text = "- none -";
      // 
      // Form1
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(710, 471);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.buttonPCM);
      this.Controls.Add(this.buttonDPCM);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.buttonHistogram);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(700, 200);
      this.Name = "Form1";
      this.Text = "066 image histogram";
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
      this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonHistogram;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Button buttonDPCM;
    private System.Windows.Forms.Button buttonPCM;
    private System.Windows.Forms.Label labelStatus;
  }
}

