namespace _086shader
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
      this.buttonOpen = new System.Windows.Forms.Button();
      this.labelFile = new System.Windows.Forms.Label();
      this.textParam = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.checkMulti = new System.Windows.Forms.CheckBox();
      this.checkSmooth = new System.Windows.Forms.CheckBox();
      this.checkWireframe = new System.Windows.Forms.CheckBox();
      this.checkTwosided = new System.Windows.Forms.CheckBox();
      this.checkVsync = new System.Windows.Forms.CheckBox();
      this.checkTexture = new System.Windows.Forms.CheckBox();
      this.checkGlobalColor = new System.Windows.Forms.CheckBox();
      this.checkShaders = new System.Windows.Forms.CheckBox();
      this.checkAmbient = new System.Windows.Forms.CheckBox();
      this.checkDiffuse = new System.Windows.Forms.CheckBox();
      this.checkSpecular = new System.Windows.Forms.CheckBox();
      this.checkPhong = new System.Windows.Forms.CheckBox();
      this.buttonLoadTexture = new System.Windows.Forms.Button();
      this.buttonExportPly = new System.Windows.Forms.Button();
      this.checkOrientation = new System.Windows.Forms.CheckBox();
      this.checkAxes = new System.Windows.Forms.CheckBox();
      this.checkCorner = new System.Windows.Forms.CheckBox();
      this.buttonStartStop = new System.Windows.Forms.Button();
      this.buttonLoadCamera = new System.Windows.Forms.Button();
      this.checkAnimation = new System.Windows.Forms.CheckBox();
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
      this.glControl1.TabIndex = 0;
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
      this.labelFps.Location = new System.Drawing.Point(702, 381);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 12;
      this.labelFps.Text = "Fps:";
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonReset.Location = new System.Drawing.Point(756, 434);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(69, 23);
      this.buttonReset.TabIndex = 26;
      this.buttonReset.Text = "Reset cam";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(15, 375);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(108, 23);
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load scene";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // labelFile
      // 
      this.labelFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelFile.AutoSize = true;
      this.labelFile.Location = new System.Drawing.Point(546, 380);
      this.labelFile.Name = "labelFile";
      this.labelFile.Size = new System.Drawing.Size(49, 13);
      this.labelFile.TabIndex = 11;
      this.labelFile.Text = "-- cube --";
      this.labelFile.MouseHover += new System.EventHandler(this.labelFile_MouseHover);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(180, 437);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(355, 20);
      this.textParam.TabIndex = 20;
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
      this.label3.TabIndex = 19;
      this.label3.Text = "Param:";
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point(15, 434);
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size(108, 23);
      this.buttonGenerate.TabIndex = 3;
      this.buttonGenerate.Text = "Generate";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
      // 
      // checkMulti
      // 
      this.checkMulti.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkMulti.AutoSize = true;
      this.checkMulti.Location = new System.Drawing.Point(544, 439);
      this.checkMulti.Name = "checkMulti";
      this.checkMulti.Size = new System.Drawing.Size(51, 17);
      this.checkMulti.TabIndex = 21;
      this.checkMulti.Text = "8 inst";
      this.checkMulti.UseVisualStyleBackColor = true;
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Checked = true;
      this.checkSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkSmooth.Location = new System.Drawing.Point(139, 380);
      this.checkSmooth.Name = "checkSmooth";
      this.checkSmooth.Size = new System.Drawing.Size(62, 17);
      this.checkSmooth.TabIndex = 4;
      this.checkSmooth.Text = "Smooth";
      this.checkSmooth.UseVisualStyleBackColor = true;
      // 
      // checkWireframe
      // 
      this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkWireframe.AutoSize = true;
      this.checkWireframe.Location = new System.Drawing.Point(204, 380);
      this.checkWireframe.Name = "checkWireframe";
      this.checkWireframe.Size = new System.Drawing.Size(48, 17);
      this.checkWireframe.TabIndex = 5;
      this.checkWireframe.Text = "Wire";
      this.checkWireframe.UseVisualStyleBackColor = true;
      // 
      // checkTwosided
      // 
      this.checkTwosided.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTwosided.AutoSize = true;
      this.checkTwosided.Checked = true;
      this.checkTwosided.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkTwosided.Location = new System.Drawing.Point(255, 380);
      this.checkTwosided.Name = "checkTwosided";
      this.checkTwosided.Size = new System.Drawing.Size(57, 17);
      this.checkTwosided.TabIndex = 6;
      this.checkTwosided.Text = "2sided";
      this.checkTwosided.UseVisualStyleBackColor = true;
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(484, 380);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(57, 17);
      this.checkVsync.TabIndex = 10;
      this.checkVsync.Text = "VSync";
      this.checkVsync.UseVisualStyleBackColor = true;
      this.checkVsync.CheckedChanged += new System.EventHandler(this.checkVsync_CheckedChanged);
      // 
      // checkTexture
      // 
      this.checkTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTexture.AutoSize = true;
      this.checkTexture.Location = new System.Drawing.Point(371, 380);
      this.checkTexture.Name = "checkTexture";
      this.checkTexture.Size = new System.Drawing.Size(44, 17);
      this.checkTexture.TabIndex = 8;
      this.checkTexture.Text = "Tex";
      this.checkTexture.UseVisualStyleBackColor = true;
      // 
      // checkGlobalColor
      // 
      this.checkGlobalColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkGlobalColor.AutoSize = true;
      this.checkGlobalColor.Location = new System.Drawing.Point(418, 380);
      this.checkGlobalColor.Name = "checkGlobalColor";
      this.checkGlobalColor.Size = new System.Drawing.Size(63, 17);
      this.checkGlobalColor.TabIndex = 9;
      this.checkGlobalColor.Text = "GlobalC";
      this.checkGlobalColor.UseVisualStyleBackColor = true;
      // 
      // checkShaders
      // 
      this.checkShaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkShaders.AutoSize = true;
      this.checkShaders.Location = new System.Drawing.Point(315, 380);
      this.checkShaders.Name = "checkShaders";
      this.checkShaders.Size = new System.Drawing.Size(53, 17);
      this.checkShaders.TabIndex = 7;
      this.checkShaders.Text = "GLSL";
      this.checkShaders.UseVisualStyleBackColor = true;
      // 
      // checkAmbient
      // 
      this.checkAmbient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAmbient.AutoSize = true;
      this.checkAmbient.Checked = true;
      this.checkAmbient.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkAmbient.Location = new System.Drawing.Point(204, 408);
      this.checkAmbient.Name = "checkAmbient";
      this.checkAmbient.Size = new System.Drawing.Size(64, 17);
      this.checkAmbient.TabIndex = 14;
      this.checkAmbient.Text = "Ambient";
      this.checkAmbient.UseVisualStyleBackColor = true;
      // 
      // checkDiffuse
      // 
      this.checkDiffuse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDiffuse.AutoSize = true;
      this.checkDiffuse.Checked = true;
      this.checkDiffuse.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkDiffuse.Location = new System.Drawing.Point(274, 408);
      this.checkDiffuse.Name = "checkDiffuse";
      this.checkDiffuse.Size = new System.Drawing.Size(59, 17);
      this.checkDiffuse.TabIndex = 15;
      this.checkDiffuse.Text = "Diffuse";
      this.checkDiffuse.UseVisualStyleBackColor = true;
      // 
      // checkSpecular
      // 
      this.checkSpecular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSpecular.AutoSize = true;
      this.checkSpecular.Checked = true;
      this.checkSpecular.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkSpecular.Location = new System.Drawing.Point(339, 408);
      this.checkSpecular.Name = "checkSpecular";
      this.checkSpecular.Size = new System.Drawing.Size(68, 17);
      this.checkSpecular.TabIndex = 16;
      this.checkSpecular.Text = "Specular";
      this.checkSpecular.UseVisualStyleBackColor = true;
      // 
      // checkPhong
      // 
      this.checkPhong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkPhong.AutoSize = true;
      this.checkPhong.Checked = true;
      this.checkPhong.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkPhong.Location = new System.Drawing.Point(139, 408);
      this.checkPhong.Name = "checkPhong";
      this.checkPhong.Size = new System.Drawing.Size(57, 17);
      this.checkPhong.TabIndex = 13;
      this.checkPhong.Text = "Phong";
      this.checkPhong.UseVisualStyleBackColor = true;
      // 
      // buttonLoadTexture
      // 
      this.buttonLoadTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoadTexture.Location = new System.Drawing.Point(15, 404);
      this.buttonLoadTexture.Name = "buttonLoadTexture";
      this.buttonLoadTexture.Size = new System.Drawing.Size(108, 23);
      this.buttonLoadTexture.TabIndex = 2;
      this.buttonLoadTexture.Text = "Load texture";
      this.buttonLoadTexture.UseVisualStyleBackColor = true;
      this.buttonLoadTexture.Click += new System.EventHandler(this.buttonLoadTexture_Click);
      // 
      // buttonExportPly
      // 
      this.buttonExportPly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonExportPly.Location = new System.Drawing.Point(756, 404);
      this.buttonExportPly.Name = "buttonExportPly";
      this.buttonExportPly.Size = new System.Drawing.Size(69, 23);
      this.buttonExportPly.TabIndex = 23;
      this.buttonExportPly.Text = "Export 3D";
      this.buttonExportPly.UseVisualStyleBackColor = true;
      this.buttonExportPly.Click += new System.EventHandler(this.buttonExportPly_Click);
      // 
      // checkOrientation
      // 
      this.checkOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkOrientation.AutoSize = true;
      this.checkOrientation.Location = new System.Drawing.Point(677, 408);
      this.checkOrientation.Name = "checkOrientation";
      this.checkOrientation.Size = new System.Drawing.Size(77, 17);
      this.checkOrientation.TabIndex = 22;
      this.checkOrientation.Text = "Orientation";
      this.checkOrientation.UseVisualStyleBackColor = true;
      // 
      // checkAxes
      // 
      this.checkAxes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAxes.AutoSize = true;
      this.checkAxes.Location = new System.Drawing.Point(418, 408);
      this.checkAxes.Name = "checkAxes";
      this.checkAxes.Size = new System.Drawing.Size(49, 17);
      this.checkAxes.TabIndex = 17;
      this.checkAxes.Text = "Axes";
      this.checkAxes.UseVisualStyleBackColor = true;
      // 
      // checkCorner
      // 
      this.checkCorner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkCorner.AutoSize = true;
      this.checkCorner.Location = new System.Drawing.Point(477, 408);
      this.checkCorner.Name = "checkCorner";
      this.checkCorner.Size = new System.Drawing.Size(116, 17);
      this.checkCorner.TabIndex = 18;
      this.checkCorner.Text = "Check corner-table";
      this.checkCorner.UseVisualStyleBackColor = true;
      // 
      // buttonStartStop
      // 
      this.buttonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonStartStop.Enabled = false;
      this.buttonStartStop.Location = new System.Drawing.Point(677, 434);
      this.buttonStartStop.Name = "buttonStartStop";
      this.buttonStartStop.Size = new System.Drawing.Size(73, 23);
      this.buttonStartStop.TabIndex = 25;
      this.buttonStartStop.Text = "Start/Stop";
      this.buttonStartStop.UseVisualStyleBackColor = true;
      this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
      // 
      // buttonLoadCamera
      // 
      this.buttonLoadCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonLoadCamera.Location = new System.Drawing.Point(598, 434);
      this.buttonLoadCamera.Name = "buttonLoadCamera";
      this.buttonLoadCamera.Size = new System.Drawing.Size(71, 23);
      this.buttonLoadCamera.TabIndex = 24;
      this.buttonLoadCamera.Text = "Load cam";
      this.buttonLoadCamera.UseVisualStyleBackColor = true;
      this.buttonLoadCamera.Click += new System.EventHandler(this.buttonLoadCamera_Click);
      // 
      // checkAnimation
      // 
      this.checkAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkAnimation.AutoSize = true;
      this.checkAnimation.Location = new System.Drawing.Point(600, 408);
      this.checkAnimation.Name = "checkAnimation";
      this.checkAnimation.Size = new System.Drawing.Size(72, 17);
      this.checkAnimation.TabIndex = 27;
      this.checkAnimation.Text = "Animation";
      this.checkAnimation.UseVisualStyleBackColor = true;
      this.checkAnimation.CheckedChanged += new System.EventHandler(this.checkAnimation_CheckedChanged);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(844, 469);
      this.Controls.Add(this.checkAnimation);
      this.Controls.Add(this.buttonLoadCamera);
      this.Controls.Add(this.buttonStartStop);
      this.Controls.Add(this.checkCorner);
      this.Controls.Add(this.checkAxes);
      this.Controls.Add(this.checkOrientation);
      this.Controls.Add(this.buttonExportPly);
      this.Controls.Add(this.buttonLoadTexture);
      this.Controls.Add(this.checkPhong);
      this.Controls.Add(this.checkSpecular);
      this.Controls.Add(this.checkDiffuse);
      this.Controls.Add(this.checkAmbient);
      this.Controls.Add(this.checkShaders);
      this.Controls.Add(this.checkGlobalColor);
      this.Controls.Add(this.checkTexture);
      this.Controls.Add(this.checkVsync);
      this.Controls.Add(this.checkTwosided);
      this.Controls.Add(this.checkWireframe);
      this.Controls.Add(this.checkSmooth);
      this.Controls.Add(this.checkMulti);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.buttonGenerate);
      this.Controls.Add(this.labelFile);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.labelFps);
      this.Controls.Add(this.glControl1);
      this.MinimumSize = new System.Drawing.Size(860, 350);
      this.Name = "Form1";
      this.Text = "086 shaders";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Label labelFile;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.CheckBox checkMulti;
    private System.Windows.Forms.CheckBox checkSmooth;
    private System.Windows.Forms.CheckBox checkWireframe;
    private System.Windows.Forms.CheckBox checkTwosided;
    private System.Windows.Forms.CheckBox checkVsync;
    private System.Windows.Forms.CheckBox checkTexture;
    private System.Windows.Forms.CheckBox checkGlobalColor;
    private System.Windows.Forms.CheckBox checkShaders;
    private System.Windows.Forms.CheckBox checkAmbient;
    private System.Windows.Forms.CheckBox checkDiffuse;
    private System.Windows.Forms.CheckBox checkSpecular;
    private System.Windows.Forms.CheckBox checkPhong;
    private System.Windows.Forms.Button buttonLoadTexture;
    private System.Windows.Forms.Button buttonExportPly;
    private System.Windows.Forms.CheckBox checkOrientation;
    private System.Windows.Forms.CheckBox checkAxes;
    private System.Windows.Forms.CheckBox checkCorner;
    private System.Windows.Forms.Button buttonStartStop;
    private System.Windows.Forms.Button buttonLoadCamera;
    private System.Windows.Forms.CheckBox checkAnimation;
  }
}

