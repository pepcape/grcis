namespace _113graph
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
      this.labelStatus = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.glControl1 = new OpenTK.GLControl();
      this.labelFps = new System.Windows.Forms.Label();
      this.checkSmooth = new System.Windows.Forms.CheckBox();
      this.checkWireframe = new System.Windows.Forms.CheckBox();
      this.textParam = new System.Windows.Forms.TextBox();
      this.checkTwosided = new System.Windows.Forms.CheckBox();
      this.checkDebug = new System.Windows.Forms.CheckBox();
      this.checkVsync = new System.Windows.Forms.CheckBox();
      this.textExpression = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonRegenerate = new System.Windows.Forms.Button();
      this.checkTexture = new System.Windows.Forms.CheckBox();
      this.checkLighting = new System.Windows.Forms.CheckBox();
      this.checkPhong = new System.Windows.Forms.CheckBox();
      this.checkGlobalColor = new System.Windows.Forms.CheckBox();
      this.checkAmbient = new System.Windows.Forms.CheckBox();
      this.checkDiffuse = new System.Windows.Forms.CheckBox();
      this.checkSpecular = new System.Windows.Forms.CheckBox();
      this.buttonReset = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(676, 417);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(37, 13);
      this.labelStatus.TabIndex = 16;
      this.labelStatus.Text = "Status";
      this.labelStatus.MouseHover += new System.EventHandler(this.labelStatus_MouseHover);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(117, 418);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 14;
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
      this.glControl1.Size = new System.Drawing.Size(802, 355);
      this.glControl1.TabIndex = 0;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
      this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
      this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
      this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
      this.glControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.control_PreviewKeyDown);
      this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
      // 
      // labelFps
      // 
      this.labelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelFps.AutoSize = true;
      this.labelFps.Location = new System.Drawing.Point(676, 450);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 20;
      this.labelFps.Text = "Fps:";
      this.labelFps.MouseHover += new System.EventHandler(this.labelFps_MouseHover);
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Location = new System.Drawing.Point(266, 385);
      this.checkSmooth.Name = "checkSmooth";
      this.checkSmooth.Size = new System.Drawing.Size(62, 17);
      this.checkSmooth.TabIndex = 5;
      this.checkSmooth.Text = "Smooth";
      this.checkSmooth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkSmooth.UseVisualStyleBackColor = true;
      // 
      // checkWireframe
      // 
      this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkWireframe.AutoSize = true;
      this.checkWireframe.Location = new System.Drawing.Point(75, 385);
      this.checkWireframe.Name = "checkWireframe";
      this.checkWireframe.Size = new System.Drawing.Size(48, 17);
      this.checkWireframe.TabIndex = 2;
      this.checkWireframe.Text = "Wire";
      this.checkWireframe.UseVisualStyleBackColor = true;
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(172, 414);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(494, 20);
      this.textParam.TabIndex = 15;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      this.textParam.MouseHover += new System.EventHandler(this.textParam_MouseHover);
      // 
      // checkTwosided
      // 
      this.checkTwosided.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTwosided.AutoSize = true;
      this.checkTwosided.Checked = true;
      this.checkTwosided.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkTwosided.Location = new System.Drawing.Point(12, 385);
      this.checkTwosided.Name = "checkTwosided";
      this.checkTwosided.Size = new System.Drawing.Size(57, 17);
      this.checkTwosided.TabIndex = 1;
      this.checkTwosided.Text = "2sided";
      this.checkTwosided.UseVisualStyleBackColor = true;
      // 
      // checkDebug
      // 
      this.checkDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkDebug.AutoSize = true;
      this.checkDebug.Location = new System.Drawing.Point(761, 385);
      this.checkDebug.Name = "checkDebug";
      this.checkDebug.Size = new System.Drawing.Size(58, 17);
      this.checkDebug.TabIndex = 12;
      this.checkDebug.Text = "Debug";
      this.checkDebug.UseVisualStyleBackColor = true;
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(699, 385);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(57, 17);
      this.checkVsync.TabIndex = 11;
      this.checkVsync.Text = "VSync";
      this.checkVsync.UseVisualStyleBackColor = true;
      this.checkVsync.CheckedChanged += new System.EventHandler(this.checkVsync_CheckedChanged);
      // 
      // textExpression
      // 
      this.textExpression.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textExpression.Location = new System.Drawing.Point(172, 446);
      this.textExpression.Name = "textExpression";
      this.textExpression.Size = new System.Drawing.Size(494, 20);
      this.textExpression.TabIndex = 19;
      this.textExpression.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textExpression_KeyPress);
      this.textExpression.MouseHover += new System.EventHandler(this.textExpression_MouseHover);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(116, 450);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(31, 13);
      this.label1.TabIndex = 18;
      this.label1.Text = "Expr:";
      // 
      // buttonRegenerate
      // 
      this.buttonRegenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRegenerate.Location = new System.Drawing.Point(12, 408);
      this.buttonRegenerate.Name = "buttonRegenerate";
      this.buttonRegenerate.Size = new System.Drawing.Size(94, 32);
      this.buttonRegenerate.TabIndex = 13;
      this.buttonRegenerate.Text = "Regenerate";
      this.buttonRegenerate.UseVisualStyleBackColor = true;
      this.buttonRegenerate.Click += new System.EventHandler(this.buttonRegenerate_Click);
      // 
      // checkTexture
      //
      this.checkTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTexture.AutoSize = true;
      this.checkTexture.Location = new System.Drawing.Point(129, 385);
      this.checkTexture.Name = "checkTexture";
      this.checkTexture.Size = new System.Drawing.Size(62, 17);
      this.checkTexture.TabIndex = 3;
      this.checkTexture.Text = "Texture";
      this.checkTexture.UseVisualStyleBackColor = true;
      //
      // checkLighting
      //
      this.checkLighting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkLighting.AutoSize = true;
      this.checkLighting.Location = new System.Drawing.Point(334, 385);
      this.checkLighting.Name = "checkLighting";
      this.checkLighting.Size = new System.Drawing.Size(63, 17);
      this.checkLighting.TabIndex = 6;
      this.checkLighting.Text = "Lighting";
      this.checkLighting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkLighting.UseVisualStyleBackColor = true;
      //
      // checkPhong
      //
      this.checkPhong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkPhong.AutoSize = true;
      this.checkPhong.Location = new System.Drawing.Point(403, 385);
      this.checkPhong.Name = "checkPhong";
      this.checkPhong.Size = new System.Drawing.Size(57, 17);
      this.checkPhong.TabIndex = 7;
      this.checkPhong.Text = "Phong";
      this.checkPhong.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkPhong.UseVisualStyleBackColor = true;
      //
      // checkGlobalColor
      //
      this.checkGlobalColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkGlobalColor.AutoSize = true;
      this.checkGlobalColor.Location = new System.Drawing.Point(197, 385);
      this.checkGlobalColor.Name = "checkGlobalColor";
      this.checkGlobalColor.Size = new System.Drawing.Size(63, 17);
      this.checkGlobalColor.TabIndex = 4;
      this.checkGlobalColor.Text = "GlobalC";
      this.checkGlobalColor.UseVisualStyleBackColor = true;
      //
      // checkAmbient
      //
      this.checkAmbient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAmbient.AutoSize = true;
      this.checkAmbient.Checked = true;
      this.checkAmbient.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkAmbient.Location = new System.Drawing.Point(466, 385);
      this.checkAmbient.Name = "checkAmbient";
      this.checkAmbient.Size = new System.Drawing.Size(64, 17);
      this.checkAmbient.TabIndex = 8;
      this.checkAmbient.Text = "Ambient";
      this.checkAmbient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkAmbient.UseVisualStyleBackColor = true;
      //
      // checkDiffuse
      //
      this.checkDiffuse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDiffuse.AutoSize = true;
      this.checkDiffuse.Checked = true;
      this.checkDiffuse.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkDiffuse.Location = new System.Drawing.Point(536, 385);
      this.checkDiffuse.Name = "checkDiffuse";
      this.checkDiffuse.Size = new System.Drawing.Size(59, 17);
      this.checkDiffuse.TabIndex = 9;
      this.checkDiffuse.Text = "Diffuse";
      this.checkDiffuse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkDiffuse.UseVisualStyleBackColor = true;
      //
      // checkSpecular
      //
      this.checkSpecular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSpecular.AutoSize = true;
      this.checkSpecular.Checked = true;
      this.checkSpecular.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkSpecular.Location = new System.Drawing.Point(601, 385);
      this.checkSpecular.Name = "checkSpecular";
      this.checkSpecular.Size = new System.Drawing.Size(68, 17);
      this.checkSpecular.TabIndex = 10;
      this.checkSpecular.Text = "Specular";
      this.checkSpecular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkSpecular.UseVisualStyleBackColor = true;
      //
      // buttonReset
      //
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonReset.Location = new System.Drawing.Point(12, 446);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(94, 23);
      this.buttonReset.TabIndex = 17;
      this.buttonReset.Text = "Reset cam";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      //
      // Form1
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(834, 481);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.checkSpecular);
      this.Controls.Add(this.checkDiffuse);
      this.Controls.Add(this.checkAmbient);
      this.Controls.Add(this.checkGlobalColor);
      this.Controls.Add(this.checkPhong);
      this.Controls.Add(this.checkLighting);
      this.Controls.Add(this.checkTexture);
      this.Controls.Add(this.buttonRegenerate);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textExpression);
      this.Controls.Add(this.checkDebug);
      this.Controls.Add(this.checkVsync);
      this.Controls.Add(this.checkTwosided);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.checkWireframe);
      this.Controls.Add(this.checkSmooth);
      this.Controls.Add(this.labelFps);
      this.Controls.Add(this.glControl1);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.labelStatus);
      this.MinimumSize = new System.Drawing.Size(840, 420);
      this.Name = "Form1";
      this.Text = "113 graph";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.Label label3;
    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.CheckBox checkSmooth;
    private System.Windows.Forms.CheckBox checkWireframe;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.CheckBox checkTwosided;
    private System.Windows.Forms.CheckBox checkDebug;
    private System.Windows.Forms.CheckBox checkVsync;
    private System.Windows.Forms.TextBox textExpression;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonRegenerate;
    private System.Windows.Forms.CheckBox checkTexture;
    private System.Windows.Forms.CheckBox checkLighting;
    private System.Windows.Forms.CheckBox checkPhong;
    private System.Windows.Forms.CheckBox checkGlobalColor;
    private System.Windows.Forms.CheckBox checkAmbient;
    private System.Windows.Forms.CheckBox checkDiffuse;
    private System.Windows.Forms.CheckBox checkSpecular;
    private System.Windows.Forms.Button buttonReset;
  }
}

