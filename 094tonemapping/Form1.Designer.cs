namespace _094tonemapping
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
      this.buttonSave = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.buttonStop = new System.Windows.Forms.Button();
      this.trackBarExp = new System.Windows.Forms.TrackBar();
      this.labelMin = new System.Windows.Forms.Label();
      this.labelMax = new System.Windows.Forms.Label();
      this.labelStatus = new System.Windows.Forms.Label();
      this.labelExpValue = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBarExp)).BeginInit();
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
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(13, 403);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(85, 23);
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load image";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonSave.Location = new System.Drawing.Point(303, 435);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(82, 23);
      this.buttonSave.TabIndex = 7;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(108, 408);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(40, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Param:";
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRedraw.Location = new System.Drawing.Point(14, 435);
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size(84, 23);
      this.buttonRedraw.TabIndex = 4;
      this.buttonRedraw.Text = "Tone-map";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler(this.buttonRedraw_Click);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point(152, 405);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(233, 20);
      this.textParam.TabIndex = 3;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      this.textParam.MouseHover += new System.EventHandler(this.textParam_MouseHover);
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point(108, 435);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(40, 23);
      this.buttonStop.TabIndex = 5;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // trackBarExp
      // 
      this.trackBarExp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.trackBarExp.LargeChange = 20;
      this.trackBarExp.Location = new System.Drawing.Point(399, 408);
      this.trackBarExp.Maximum = 400;
      this.trackBarExp.Name = "trackBarExp";
      this.trackBarExp.Size = new System.Drawing.Size(299, 45);
      this.trackBarExp.SmallChange = 0;
      this.trackBarExp.TabIndex = 8;
      this.trackBarExp.TickFrequency = 5;
      this.trackBarExp.TickStyle = System.Windows.Forms.TickStyle.None;
      this.trackBarExp.Value = 200;
      this.trackBarExp.ValueChanged += new System.EventHandler(this.trackBarExp_ValueChanged);
      // 
      // labelMin
      // 
      this.labelMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelMin.AutoSize = true;
      this.labelMin.Location = new System.Drawing.Point(405, 438);
      this.labelMin.Name = "labelMin";
      this.labelMin.Size = new System.Drawing.Size(23, 13);
      this.labelMin.TabIndex = 9;
      this.labelMin.Text = "min";
      // 
      // labelMax
      // 
      this.labelMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelMax.Location = new System.Drawing.Point(642, 438);
      this.labelMax.Name = "labelMax";
      this.labelMax.Size = new System.Drawing.Size(50, 13);
      this.labelMax.TabIndex = 11;
      this.labelMax.Text = "max";
      this.labelMax.TextAlign = System.Drawing.ContentAlignment.BottomRight;
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(161, 440);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(40, 13);
      this.labelStatus.TabIndex = 6;
      this.labelStatus.Text = "Status:";
      this.labelStatus.MouseHover += new System.EventHandler(this.labelStatus_MouseHover);
      // 
      // labelExpValue
      // 
      this.labelExpValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelExpValue.AutoSize = true;
      this.labelExpValue.Location = new System.Drawing.Point(538, 439);
      this.labelExpValue.Name = "labelExpValue";
      this.labelExpValue.Size = new System.Drawing.Size(21, 13);
      this.labelExpValue.TabIndex = 10;
      this.labelExpValue.Text = "val";
      this.labelExpValue.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(710, 466);
      this.Controls.Add(this.labelExpValue);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.labelMax);
      this.Controls.Add(this.labelMin);
      this.Controls.Add(this.trackBarExp);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.buttonRedraw);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(600, 300);
      this.Name = "Form1";
      this.Text = "094 tone mapping";
      this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBarExp)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.TrackBar trackBarExp;
    private System.Windows.Forms.Label labelMin;
    private System.Windows.Forms.Label labelMax;
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.Label labelExpValue;
  }
}

