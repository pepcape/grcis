namespace _090opencl
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
      this.glControl1 = new OpenTK.GLControl();
      this.labelFps = new System.Windows.Forms.Label();
      this.buttonReset = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.checkDouble = new System.Windows.Forms.CheckBox();
      this.checkVsync = new System.Windows.Forms.CheckBox();
      this.checkOpenCL = new System.Windows.Forms.CheckBox();
      this.comboBoxPlatform = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBoxDevice = new System.Windows.Forms.ComboBox();
      this.checkPalette = new System.Windows.Forms.CheckBox();
      this.checkBigGroup = new System.Windows.Forms.CheckBox();
      this.labelSize = new System.Windows.Forms.Label();
      this.checkInterop = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // glControl1
      // 
      this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.glControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.glControl1.BackColor = System.Drawing.Color.Black;
      this.glControl1.Location = new System.Drawing.Point(13, 12);
      this.glControl1.Name = "glControl1";
      this.glControl1.Size = new System.Drawing.Size(812, 350);
      this.glControl1.TabIndex = 17;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
      this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
      this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
      this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
      // 
      // labelFps
      // 
      this.labelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelFps.AutoSize = true;
      this.labelFps.Location = new System.Drawing.Point(527, 381);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonReset.Location = new System.Drawing.Point(767, 435);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(60, 23);
      this.buttonReset.TabIndex = 22;
      this.buttonReset.Text = "Reset";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point(61, 437);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(452, 20);
      this.textParam.TabIndex = 29;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(11, 440);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 28;
      this.label3.Text = "Param:";
      // 
      // checkDouble
      // 
      this.checkDouble.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDouble.AutoSize = true;
      this.checkDouble.Location = new System.Drawing.Point(528, 441);
      this.checkDouble.Name = "checkDouble";
      this.checkDouble.Size = new System.Drawing.Size(60, 17);
      this.checkDouble.TabIndex = 31;
      this.checkDouble.Text = "Double";
      this.checkDouble.UseVisualStyleBackColor = true;
      this.checkDouble.CheckedChanged += new System.EventHandler(this.checkDouble_CheckedChanged);
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(640, 441);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(57, 17);
      this.checkVsync.TabIndex = 34;
      this.checkVsync.Text = "VSync";
      this.checkVsync.UseVisualStyleBackColor = true;
      this.checkVsync.CheckedChanged += new System.EventHandler(this.checkVsync_CheckedChanged);
      // 
      // checkOpenCL
      // 
      this.checkOpenCL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkOpenCL.AutoSize = true;
      this.checkOpenCL.Location = new System.Drawing.Point(528, 408);
      this.checkOpenCL.Name = "checkOpenCL";
      this.checkOpenCL.Size = new System.Drawing.Size(65, 17);
      this.checkOpenCL.TabIndex = 37;
      this.checkOpenCL.Text = "OpenCL";
      this.checkOpenCL.UseVisualStyleBackColor = true;
      this.checkOpenCL.CheckedChanged += new System.EventHandler(this.checkOpenCL_CheckedChanged);
      // 
      // comboBoxPlatform
      // 
      this.comboBoxPlatform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboBoxPlatform.FormattingEnabled = true;
      this.comboBoxPlatform.Location = new System.Drawing.Point(61, 376);
      this.comboBoxPlatform.Name = "comboBoxPlatform";
      this.comboBoxPlatform.Size = new System.Drawing.Size(452, 21);
      this.comboBoxPlatform.TabIndex = 38;
      this.comboBoxPlatform.SelectedIndexChanged += new System.EventHandler(this.comboBoxPlatform_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(11, 379);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(48, 13);
      this.label1.TabIndex = 39;
      this.label1.Text = "Platform:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 409);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(44, 13);
      this.label2.TabIndex = 41;
      this.label2.Text = "Device:";
      // 
      // comboBoxDevice
      // 
      this.comboBoxDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboBoxDevice.FormattingEnabled = true;
      this.comboBoxDevice.Location = new System.Drawing.Point(61, 406);
      this.comboBoxDevice.Name = "comboBoxDevice";
      this.comboBoxDevice.Size = new System.Drawing.Size(452, 21);
      this.comboBoxDevice.TabIndex = 40;
      this.comboBoxDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevice_SelectedIndexChanged);
      // 
      // checkPalette
      // 
      this.checkPalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkPalette.AutoSize = true;
      this.checkPalette.Checked = true;
      this.checkPalette.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkPalette.Location = new System.Drawing.Point(702, 441);
      this.checkPalette.Name = "checkPalette";
      this.checkPalette.Size = new System.Drawing.Size(59, 17);
      this.checkPalette.TabIndex = 42;
      this.checkPalette.Text = "Palette";
      this.checkPalette.UseVisualStyleBackColor = true;
      // 
      // checkBigGroup
      // 
      this.checkBigGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkBigGroup.AutoSize = true;
      this.checkBigGroup.Location = new System.Drawing.Point(595, 408);
      this.checkBigGroup.Name = "checkBigGroup";
      this.checkBigGroup.Size = new System.Drawing.Size(76, 17);
      this.checkBigGroup.TabIndex = 43;
      this.checkBigGroup.Text = "Big groups";
      this.checkBigGroup.UseVisualStyleBackColor = true;
      this.checkBigGroup.CheckedChanged += new System.EventHandler(this.checkBigGroup_CheckedChanged);
      // 
      // labelSize
      // 
      this.labelSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelSize.AutoSize = true;
      this.labelSize.Location = new System.Drawing.Point(675, 409);
      this.labelSize.Name = "labelSize";
      this.labelSize.Size = new System.Drawing.Size(30, 13);
      this.labelSize.TabIndex = 44;
      this.labelSize.Text = "Size:";
      // 
      // checkInterop
      // 
      this.checkInterop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkInterop.AutoSize = true;
      this.checkInterop.Location = new System.Drawing.Point(592, 441);
      this.checkInterop.Name = "checkInterop";
      this.checkInterop.Size = new System.Drawing.Size(40, 17);
      this.checkInterop.TabIndex = 45;
      this.checkInterop.Text = "GL";
      this.checkInterop.UseVisualStyleBackColor = true;
      this.checkInterop.CheckedChanged += new System.EventHandler(this.checkInterop_CheckedChanged);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(844, 469);
      this.Controls.Add(this.checkInterop);
      this.Controls.Add(this.labelSize);
      this.Controls.Add(this.checkBigGroup);
      this.Controls.Add(this.checkPalette);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.comboBoxDevice);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.comboBoxPlatform);
      this.Controls.Add(this.checkOpenCL);
      this.Controls.Add(this.checkVsync);
      this.Controls.Add(this.checkDouble);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.labelFps);
      this.Controls.Add(this.glControl1);
      this.MinimumSize = new System.Drawing.Size(860, 350);
      this.Name = "Form1";
      this.Text = "090 OpenCL";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox checkDouble;
    private System.Windows.Forms.CheckBox checkVsync;
    private System.Windows.Forms.CheckBox checkOpenCL;
    private System.Windows.Forms.ComboBox comboBoxPlatform;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox comboBoxDevice;
    private System.Windows.Forms.CheckBox checkPalette;
    private System.Windows.Forms.CheckBox checkBigGroup;
    private System.Windows.Forms.Label labelSize;
    private System.Windows.Forms.CheckBox checkInterop;
  }
}

