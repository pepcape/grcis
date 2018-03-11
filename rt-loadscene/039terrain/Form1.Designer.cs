namespace _039terrain
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

    private class OGLAA : OpenTK.GLControl
    {
        public OGLAA()
            :base(new OpenTK.Graphics.GraphicsMode(
                OpenTK.Graphics.GraphicsMode.Default.ColorFormat,
                OpenTK.Graphics.GraphicsMode.Default.Depth,
                OpenTK.Graphics.GraphicsMode.Default.Stencil,
                4), 3, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible)
        {
        }
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
      this.buttonSave = new System.Windows.Forms.Button();
      this.upDownIterations = new System.Windows.Forms.NumericUpDown();
      this.labelStatus = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.buttonRegenerate = new System.Windows.Forms.Button();
      this.upDownRoughness = new System.Windows.Forms.NumericUpDown();
      this.buttonResetCam = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.checkTexture = new System.Windows.Forms.CheckBox();
      this.checkVsync = new System.Windows.Forms.CheckBox();
      this.checkWireframe = new System.Windows.Forms.CheckBox();
      this.checkSmooth = new System.Windows.Forms.CheckBox();
      this.checkAnim = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.upDownIterations)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownRoughness)).BeginInit();
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
      this.glControl1.Size = new System.Drawing.Size(774, 460);
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
      this.labelFps.Location = new System.Drawing.Point(545, 496);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Enabled = false;
      this.buttonSave.Location = new System.Drawing.Point(683, 521);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(104, 23);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // upDownIterations
      // 
      this.upDownIterations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.upDownIterations.Location = new System.Drawing.Point(201, 493);
      this.upDownIterations.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.upDownIterations.Name = "upDownIterations";
      this.upDownIterations.Size = new System.Drawing.Size(55, 20);
      this.upDownIterations.TabIndex = 19;
      this.upDownIterations.ValueChanged += new System.EventHandler(this.upDownIterations_ValueChanged);
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(14, 527);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(35, 13);
      this.labelStatus.TabIndex = 22;
      this.labelStatus.Text = "status";
      this.labelStatus.MouseHover += new System.EventHandler(this.labelStatus_MouseHover);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(270, 527);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(45, 13);
      this.label2.TabIndex = 23;
      this.label2.Text = "Params:";
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(134, 496);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(53, 13);
      this.label4.TabIndex = 27;
      this.label4.Text = "Iterations:";
      // 
      // label5
      // 
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(134, 526);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(64, 13);
      this.label5.TabIndex = 28;
      this.label5.Text = "Roughness:";
      // 
      // buttonRegenerate
      // 
      this.buttonRegenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRegenerate.Location = new System.Drawing.Point(13, 491);
      this.buttonRegenerate.Name = "buttonRegenerate";
      this.buttonRegenerate.Size = new System.Drawing.Size(111, 23);
      this.buttonRegenerate.TabIndex = 33;
      this.buttonRegenerate.Text = "Regenerate terrain";
      this.buttonRegenerate.UseVisualStyleBackColor = true;
      this.buttonRegenerate.Click += new System.EventHandler(this.buttonRegenerate_Click);
      // 
      // upDownRoughness
      // 
      this.upDownRoughness.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.upDownRoughness.DecimalPlaces = 3;
      this.upDownRoughness.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.upDownRoughness.Location = new System.Drawing.Point(201, 524);
      this.upDownRoughness.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.upDownRoughness.Name = "upDownRoughness";
      this.upDownRoughness.Size = new System.Drawing.Size(55, 20);
      this.upDownRoughness.TabIndex = 26;
      this.upDownRoughness.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      // 
      // buttonResetCam
      // 
      this.buttonResetCam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonResetCam.Location = new System.Drawing.Point(683, 491);
      this.buttonResetCam.Name = "buttonResetCam";
      this.buttonResetCam.Size = new System.Drawing.Size(104, 23);
      this.buttonResetCam.TabIndex = 34;
      this.buttonResetCam.Text = "Reset cam";
      this.buttonResetCam.UseVisualStyleBackColor = true;
      this.buttonResetCam.Click += new System.EventHandler(this.buttonResetCam_Click);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(321, 524);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(347, 20);
      this.textParam.TabIndex = 35;
      // 
      // checkTexture
      // 
      this.checkTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTexture.AutoSize = true;
      this.checkTexture.Checked = true;
      this.checkTexture.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkTexture.Location = new System.Drawing.Point(387, 496);
      this.checkTexture.Name = "checkTexture";
      this.checkTexture.Size = new System.Drawing.Size(44, 17);
      this.checkTexture.TabIndex = 39;
      this.checkTexture.Text = "Tex";
      this.checkTexture.UseVisualStyleBackColor = true;
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(433, 496);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(57, 17);
      this.checkVsync.TabIndex = 38;
      this.checkVsync.Text = "VSync";
      this.checkVsync.UseVisualStyleBackColor = true;
      this.checkVsync.CheckedChanged += new System.EventHandler(this.checkVsync_CheckedChanged);
      // 
      // checkWireframe
      // 
      this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkWireframe.AutoSize = true;
      this.checkWireframe.Location = new System.Drawing.Point(337, 496);
      this.checkWireframe.Name = "checkWireframe";
      this.checkWireframe.Size = new System.Drawing.Size(48, 17);
      this.checkWireframe.TabIndex = 37;
      this.checkWireframe.Text = "Wire";
      this.checkWireframe.UseVisualStyleBackColor = true;
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Checked = true;
      this.checkSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkSmooth.Location = new System.Drawing.Point(273, 496);
      this.checkSmooth.Name = "checkSmooth";
      this.checkSmooth.Size = new System.Drawing.Size(62, 17);
      this.checkSmooth.TabIndex = 36;
      this.checkSmooth.Text = "Smooth";
      this.checkSmooth.UseVisualStyleBackColor = true;
      // 
      // checkAnim
      // 
      this.checkAnim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAnim.AutoSize = true;
      this.checkAnim.Location = new System.Drawing.Point(492, 496);
      this.checkAnim.Name = "checkAnim";
      this.checkAnim.Size = new System.Drawing.Size(49, 17);
      this.checkAnim.TabIndex = 40;
      this.checkAnim.Text = "Anim";
      this.checkAnim.UseVisualStyleBackColor = true;
      this.checkAnim.CheckedChanged += new System.EventHandler(this.checkAnim_CheckedChanged);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(801, 556);
      this.Controls.Add(this.checkAnim);
      this.Controls.Add(this.checkTexture);
      this.Controls.Add(this.checkVsync);
      this.Controls.Add(this.checkWireframe);
      this.Controls.Add(this.checkSmooth);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.buttonResetCam);
      this.Controls.Add(this.buttonRegenerate);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.upDownRoughness);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.upDownIterations);
      this.Controls.Add(this.labelFps);
      this.Controls.Add(this.glControl1);
      this.Controls.Add(this.buttonSave);
      this.MinimumSize = new System.Drawing.Size(810, 300);
      this.Name = "Form1";
      this.Text = "039 terrain";
      ((System.ComponentModel.ISupportInitialize)(this.upDownIterations)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownRoughness)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown upDownIterations;
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button buttonRegenerate;
    private System.Windows.Forms.NumericUpDown upDownRoughness;
    private System.Windows.Forms.Button buttonResetCam;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.CheckBox checkTexture;
    private System.Windows.Forms.CheckBox checkVsync;
    private System.Windows.Forms.CheckBox checkWireframe;
    private System.Windows.Forms.CheckBox checkSmooth;
    private System.Windows.Forms.CheckBox checkAnim;
  }
}

