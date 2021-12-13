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
      this.SuspendLayout();
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(636, 417);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(37, 13);
      this.labelStatus.TabIndex = 6;
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
      this.glControl1.Size = new System.Drawing.Size(762, 355);
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
      this.glControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.control_PreviewKeyDown);
      this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
      // 
      // labelFps
      // 
      this.labelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelFps.AutoSize = true;
      this.labelFps.Location = new System.Drawing.Point(636, 450);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      this.labelFps.MouseHover += new System.EventHandler(this.labelFps_MouseHover);
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Location = new System.Drawing.Point(13, 385);
      this.checkSmooth.Name = "checkSmooth";
      this.checkSmooth.Size = new System.Drawing.Size(62, 17);
      this.checkSmooth.TabIndex = 21;
      this.checkSmooth.Text = "Smooth";
      this.checkSmooth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkSmooth.UseVisualStyleBackColor = true;
      // 
      // checkWireframe
      // 
      this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkWireframe.AutoSize = true;
      this.checkWireframe.Location = new System.Drawing.Point(80, 385);
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
      this.textParam.Location = new System.Drawing.Point(172, 414);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(454, 20);
      this.textParam.TabIndex = 23;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      this.textParam.MouseHover += new System.EventHandler(this.textParam_MouseHover);
      // 
      // checkTwosided
      // 
      this.checkTwosided.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTwosided.AutoSize = true;
      this.checkTwosided.Checked = true;
      this.checkTwosided.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkTwosided.Location = new System.Drawing.Point(133, 385);
      this.checkTwosided.Name = "checkTwosided";
      this.checkTwosided.Size = new System.Drawing.Size(57, 17);
      this.checkTwosided.TabIndex = 24;
      this.checkTwosided.Text = "2sided";
      this.checkTwosided.UseVisualStyleBackColor = true;
      // 
      // checkDebug
      // 
      this.checkDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDebug.AutoSize = true;
      this.checkDebug.Location = new System.Drawing.Point(473, 385);
      this.checkDebug.Name = "checkDebug";
      this.checkDebug.Size = new System.Drawing.Size(58, 17);
      this.checkDebug.TabIndex = 47;
      this.checkDebug.Text = "Debug";
      this.checkDebug.UseVisualStyleBackColor = true;
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(411, 385);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(57, 17);
      this.checkVsync.TabIndex = 46;
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
      this.textExpression.Size = new System.Drawing.Size(454, 20);
      this.textExpression.TabIndex = 48;
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
      this.label1.TabIndex = 49;
      this.label1.Text = "Expr:";
      // 
      // buttonRegenerate
      // 
      this.buttonRegenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRegenerate.Location = new System.Drawing.Point(13, 414);
      this.buttonRegenerate.Name = "buttonRegenerate";
      this.buttonRegenerate.Size = new System.Drawing.Size(94, 54);
      this.buttonRegenerate.TabIndex = 50;
      this.buttonRegenerate.Text = "Regenerate";
      this.buttonRegenerate.UseVisualStyleBackColor = true;
      this.buttonRegenerate.Click += new System.EventHandler(this.buttonRegenerate_Click);
      // 
      // checkTexture
      //
      this.checkTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkTexture.AutoSize = true;
      this.checkTexture.Location = new System.Drawing.Point(196, 385);
      this.checkTexture.Name = "checkTexture";
      this.checkTexture.Size = new System.Drawing.Size(62, 17);
      this.checkTexture.TabIndex = 51;
      this.checkTexture.Text = "Texture";
      this.checkTexture.UseVisualStyleBackColor = true;
      //
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(794, 481);
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
      this.MinimumSize = new System.Drawing.Size(740, 400);
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
  }
}

