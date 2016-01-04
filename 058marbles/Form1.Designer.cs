namespace _058marbles
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
      this.buttonResetSim = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.glControl1 = new OpenTK.GLControl();
      this.labelFps = new System.Windows.Forms.Label();
      this.checkSlow = new System.Windows.Forms.CheckBox();
      this.checkWireframe = new System.Windows.Forms.CheckBox();
      this.textParam = new System.Windows.Forms.TextBox();
      this.checkVsync = new System.Windows.Forms.CheckBox();
      this.buttonStart = new System.Windows.Forms.Button();
      this.buttonUpdate = new System.Windows.Forms.Button();
      this.buttonReset = new System.Windows.Forms.Button();
      this.labelStat = new System.Windows.Forms.Label();
      this.checkTexture = new System.Windows.Forms.CheckBox();
      this.checkGlobalColor = new System.Windows.Forms.CheckBox();
      this.checkNormals = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // buttonResetSim
      // 
      this.buttonResetSim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonResetSim.Location = new System.Drawing.Point(13, 406);
      this.buttonResetSim.Name = "buttonResetSim";
      this.buttonResetSim.Size = new System.Drawing.Size(84, 23);
      this.buttonResetSim.TabIndex = 11;
      this.buttonResetSim.Text = "Reset sim";
      this.buttonResetSim.UseVisualStyleBackColor = true;
      this.buttonResetSim.Click += new System.EventHandler(this.buttonResetSim_Click);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(205, 411);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 13;
      this.label3.Text = "Param:";
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
      this.glControl1.Size = new System.Drawing.Size(819, 343);
      this.glControl1.TabIndex = 17;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
      this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
      this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
      this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
      this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
      // 
      // labelFps
      // 
      this.labelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelFps.AutoSize = true;
      this.labelFps.Location = new System.Drawing.Point(721, 376);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // checkSlow
      // 
      this.checkSlow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSlow.AutoSize = true;
      this.checkSlow.Location = new System.Drawing.Point(111, 375);
      this.checkSlow.Name = "checkSlow";
      this.checkSlow.Size = new System.Drawing.Size(49, 17);
      this.checkSlow.TabIndex = 21;
      this.checkSlow.Text = "Slow";
      this.checkSlow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkSlow.UseVisualStyleBackColor = true;
      // 
      // checkWireframe
      // 
      this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkWireframe.AutoSize = true;
      this.checkWireframe.Location = new System.Drawing.Point(342, 375);
      this.checkWireframe.Name = "checkWireframe";
      this.checkWireframe.Size = new System.Drawing.Size(48, 17);
      this.checkWireframe.TabIndex = 22;
      this.checkWireframe.Text = "Wire";
      this.checkWireframe.UseVisualStyleBackColor = true;
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(250, 408);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(482, 20);
      this.textParam.TabIndex = 23;
      this.textParam.Text = "n=1000";
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(396, 375);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(57, 17);
      this.checkVsync.TabIndex = 24;
      this.checkVsync.Text = "VSync";
      this.checkVsync.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkVsync.UseVisualStyleBackColor = true;
      this.checkVsync.CheckedChanged += new System.EventHandler(this.checkVsync_CheckedChanged);
      // 
      // buttonStart
      // 
      this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStart.Location = new System.Drawing.Point(13, 371);
      this.buttonStart.Name = "buttonStart";
      this.buttonStart.Size = new System.Drawing.Size(84, 23);
      this.buttonStart.TabIndex = 25;
      this.buttonStart.Text = "Start / stop";
      this.buttonStart.UseVisualStyleBackColor = true;
      this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
      // 
      // buttonUpdate
      // 
      this.buttonUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonUpdate.Location = new System.Drawing.Point(111, 406);
      this.buttonUpdate.Name = "buttonUpdate";
      this.buttonUpdate.Size = new System.Drawing.Size(84, 23);
      this.buttonUpdate.TabIndex = 26;
      this.buttonUpdate.Text = "Update sim";
      this.buttonUpdate.UseVisualStyleBackColor = true;
      this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonReset.Location = new System.Drawing.Point(748, 406);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(84, 23);
      this.buttonReset.TabIndex = 27;
      this.buttonReset.Text = "Reset view";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      // 
      // labelStat
      // 
      this.labelStat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelStat.AutoSize = true;
      this.labelStat.Location = new System.Drawing.Point(466, 376);
      this.labelStat.Name = "labelStat";
      this.labelStat.Size = new System.Drawing.Size(86, 13);
      this.labelStat.TabIndex = 28;
      this.labelStat.Text = "-- no simulation --";
      // 
      // checkTexture
      // 
      this.checkTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTexture.AutoSize = true;
      this.checkTexture.Location = new System.Drawing.Point(166, 375);
      this.checkTexture.Name = "checkTexture";
      this.checkTexture.Size = new System.Drawing.Size(44, 17);
      this.checkTexture.TabIndex = 29;
      this.checkTexture.Text = "Tex";
      this.checkTexture.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkTexture.UseVisualStyleBackColor = true;
      // 
      // checkGlobalColor
      // 
      this.checkGlobalColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkGlobalColor.AutoSize = true;
      this.checkGlobalColor.Location = new System.Drawing.Point(216, 375);
      this.checkGlobalColor.Name = "checkGlobalColor";
      this.checkGlobalColor.Size = new System.Drawing.Size(63, 17);
      this.checkGlobalColor.TabIndex = 30;
      this.checkGlobalColor.Text = "GlobalC";
      this.checkGlobalColor.UseVisualStyleBackColor = true;
      // 
      // checkNormals
      // 
      this.checkNormals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkNormals.AutoSize = true;
      this.checkNormals.Location = new System.Drawing.Point(285, 375);
      this.checkNormals.Name = "checkNormals";
      this.checkNormals.Size = new System.Drawing.Size(51, 17);
      this.checkNormals.TabIndex = 31;
      this.checkNormals.Text = "Norm";
      this.checkNormals.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(844, 439);
      this.Controls.Add(this.checkNormals);
      this.Controls.Add(this.checkGlobalColor);
      this.Controls.Add(this.checkTexture);
      this.Controls.Add(this.labelStat);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.buttonUpdate);
      this.Controls.Add(this.buttonStart);
      this.Controls.Add(this.checkVsync);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.checkWireframe);
      this.Controls.Add(this.checkSlow);
      this.Controls.Add(this.labelFps);
      this.Controls.Add(this.glControl1);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.buttonResetSim);
      this.MinimumSize = new System.Drawing.Size(860, 300);
      this.Name = "Form1";
      this.Text = "058 marbles";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonResetSim;
    private System.Windows.Forms.Label label3;
    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.CheckBox checkSlow;
    public System.Windows.Forms.CheckBox checkWireframe;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.CheckBox checkVsync;
    private System.Windows.Forms.Button buttonStart;
    private System.Windows.Forms.Button buttonUpdate;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.Label labelStat;
    public System.Windows.Forms.CheckBox checkTexture;
    public System.Windows.Forms.CheckBox checkGlobalColor;
    public System.Windows.Forms.CheckBox checkNormals;
  }
}

