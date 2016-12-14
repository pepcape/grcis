namespace _096puzzle
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
      this.buttonResetCam = new System.Windows.Forms.Button();
      this.labelStatus = new System.Windows.Forms.Label();
      this.textParam = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.buttonUpdate = new System.Windows.Forms.Button();
      this.checkSmooth = new System.Windows.Forms.CheckBox();
      this.checkWireframe = new System.Windows.Forms.CheckBox();
      this.checkTwosided = new System.Windows.Forms.CheckBox();
      this.checkVsync = new System.Windows.Forms.CheckBox();
      this.checkTexture = new System.Windows.Forms.CheckBox();
      this.buttonResetSim = new System.Windows.Forms.Button();
      this.checkDebug = new System.Windows.Forms.CheckBox();
      this.checkSlow = new System.Windows.Forms.CheckBox();
      this.buttonStart = new System.Windows.Forms.Button();
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
      this.labelFps.Location = new System.Drawing.Point(691, 409);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // buttonResetCam
      // 
      this.buttonResetCam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonResetCam.Location = new System.Drawing.Point(741, 434);
      this.buttonResetCam.Name = "buttonResetCam";
      this.buttonResetCam.Size = new System.Drawing.Size(84, 23);
      this.buttonResetCam.TabIndex = 22;
      this.buttonResetCam.Text = "Reset cam";
      this.buttonResetCam.UseVisualStyleBackColor = true;
      this.buttonResetCam.Click += new System.EventHandler(this.buttonResetCam_Click);
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(520, 409);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(38, 13);
      this.labelStatus.TabIndex = 26;
      this.labelStatus.Text = "-- init --";
      this.labelStatus.MouseHover += new System.EventHandler(this.labelStatus_MouseHover);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(180, 436);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(549, 20);
      this.textParam.TabIndex = 29;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      this.textParam.MouseHover += new System.EventHandler(this.textParam_MouseHover);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(135, 440);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 28;
      this.label3.Text = "Param:";
      // 
      // buttonUpdate
      // 
      this.buttonUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonUpdate.Location = new System.Drawing.Point(15, 434);
      this.buttonUpdate.Name = "buttonUpdate";
      this.buttonUpdate.Size = new System.Drawing.Size(108, 23);
      this.buttonUpdate.TabIndex = 27;
      this.buttonUpdate.Text = "Update sim";
      this.buttonUpdate.UseVisualStyleBackColor = true;
      this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Checked = true;
      this.checkSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkSmooth.Location = new System.Drawing.Point(139, 407);
      this.checkSmooth.Name = "checkSmooth";
      this.checkSmooth.Size = new System.Drawing.Size(62, 17);
      this.checkSmooth.TabIndex = 31;
      this.checkSmooth.Text = "Smooth";
      this.checkSmooth.UseVisualStyleBackColor = true;
      // 
      // checkWireframe
      // 
      this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkWireframe.AutoSize = true;
      this.checkWireframe.Location = new System.Drawing.Point(210, 407);
      this.checkWireframe.Name = "checkWireframe";
      this.checkWireframe.Size = new System.Drawing.Size(48, 17);
      this.checkWireframe.TabIndex = 32;
      this.checkWireframe.Text = "Wire";
      this.checkWireframe.UseVisualStyleBackColor = true;
      // 
      // checkTwosided
      // 
      this.checkTwosided.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTwosided.AutoSize = true;
      this.checkTwosided.Checked = true;
      this.checkTwosided.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkTwosided.Location = new System.Drawing.Point(267, 407);
      this.checkTwosided.Name = "checkTwosided";
      this.checkTwosided.Size = new System.Drawing.Size(57, 17);
      this.checkTwosided.TabIndex = 33;
      this.checkTwosided.Text = "2sided";
      this.checkTwosided.UseVisualStyleBackColor = true;
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(386, 407);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(57, 17);
      this.checkVsync.TabIndex = 34;
      this.checkVsync.Text = "VSync";
      this.checkVsync.UseVisualStyleBackColor = true;
      this.checkVsync.CheckedChanged += new System.EventHandler(this.checkVsync_CheckedChanged);
      // 
      // checkTexture
      // 
      this.checkTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTexture.AutoSize = true;
      this.checkTexture.Location = new System.Drawing.Point(333, 407);
      this.checkTexture.Name = "checkTexture";
      this.checkTexture.Size = new System.Drawing.Size(44, 17);
      this.checkTexture.TabIndex = 35;
      this.checkTexture.Text = "Tex";
      this.checkTexture.UseVisualStyleBackColor = true;
      // 
      // buttonResetSim
      // 
      this.buttonResetSim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonResetSim.Location = new System.Drawing.Point(15, 404);
      this.buttonResetSim.Name = "buttonResetSim";
      this.buttonResetSim.Size = new System.Drawing.Size(108, 23);
      this.buttonResetSim.TabIndex = 42;
      this.buttonResetSim.Text = "Reset sim";
      this.buttonResetSim.UseVisualStyleBackColor = true;
      this.buttonResetSim.Click += new System.EventHandler(this.buttonResetSim_Click);
      // 
      // checkDebug
      // 
      this.checkDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDebug.AutoSize = true;
      this.checkDebug.Location = new System.Drawing.Point(452, 407);
      this.checkDebug.Name = "checkDebug";
      this.checkDebug.Size = new System.Drawing.Size(58, 17);
      this.checkDebug.TabIndex = 45;
      this.checkDebug.Text = "Debug";
      this.checkDebug.UseVisualStyleBackColor = true;
      // 
      // checkSlow
      // 
      this.checkSlow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSlow.AutoSize = true;
      this.checkSlow.Location = new System.Drawing.Point(139, 377);
      this.checkSlow.Name = "checkSlow";
      this.checkSlow.Size = new System.Drawing.Size(49, 17);
      this.checkSlow.TabIndex = 46;
      this.checkSlow.Text = "Slow";
      this.checkSlow.UseVisualStyleBackColor = true;
      // 
      // buttonStart
      // 
      this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStart.Location = new System.Drawing.Point(15, 372);
      this.buttonStart.Name = "buttonStart";
      this.buttonStart.Size = new System.Drawing.Size(108, 23);
      this.buttonStart.TabIndex = 47;
      this.buttonStart.Text = "Start / stop";
      this.buttonStart.UseVisualStyleBackColor = true;
      this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(844, 469);
      this.Controls.Add(this.buttonStart);
      this.Controls.Add(this.checkSlow);
      this.Controls.Add(this.checkDebug);
      this.Controls.Add(this.buttonResetSim);
      this.Controls.Add(this.checkTexture);
      this.Controls.Add(this.checkVsync);
      this.Controls.Add(this.checkTwosided);
      this.Controls.Add(this.checkWireframe);
      this.Controls.Add(this.checkSmooth);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.buttonUpdate);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.buttonResetCam);
      this.Controls.Add(this.labelFps);
      this.Controls.Add(this.glControl1);
      this.MinimumSize = new System.Drawing.Size(860, 350);
      this.Name = "Form1";
      this.Text = "096 puzzle";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Button buttonResetCam;
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button buttonUpdate;
    private System.Windows.Forms.CheckBox checkSmooth;
    private System.Windows.Forms.CheckBox checkWireframe;
    private System.Windows.Forms.CheckBox checkTwosided;
    private System.Windows.Forms.CheckBox checkVsync;
    private System.Windows.Forms.CheckBox checkTexture;
    private System.Windows.Forms.Button buttonResetSim;
    private System.Windows.Forms.CheckBox checkDebug;
    private System.Windows.Forms.CheckBox checkSlow;
    private System.Windows.Forms.Button buttonStart;
  }
}

