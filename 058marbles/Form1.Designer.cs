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
      this.buttonInit = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.glControl1 = new OpenTK.GLControl();
      this.labelFps = new System.Windows.Forms.Label();
      this.checkSmooth = new System.Windows.Forms.CheckBox();
      this.checkWireframe = new System.Windows.Forms.CheckBox();
      this.textParam = new System.Windows.Forms.TextBox();
      this.checkCamera = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // buttonInit
      // 
      this.buttonInit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonInit.Location = new System.Drawing.Point( 13, 372 );
      this.buttonInit.Name = "buttonInit";
      this.buttonInit.Size = new System.Drawing.Size( 73, 23 );
      this.buttonInit.TabIndex = 11;
      this.buttonInit.Text = "Init";
      this.buttonInit.UseVisualStyleBackColor = true;
      this.buttonInit.Click += new System.EventHandler( this.buttonInit_Click );
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 99, 378 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 40, 13 );
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
      this.glControl1.Location = new System.Drawing.Point( 13, 12 );
      this.glControl1.Name = "glControl1";
      this.glControl1.Size = new System.Drawing.Size( 680, 343 );
      this.glControl1.TabIndex = 17;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler( this.glControl1_Load );
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl1_Paint );
      this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler( this.glControl1_KeyDown );
      this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler( this.glControl1_KeyUp );
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseDown );
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseMove );
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseUp );
      this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseWheel );
      this.glControl1.Resize += new System.EventHandler( this.glControl1_Resize );
      // 
      // labelFps
      // 
      this.labelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelFps.AutoSize = true;
      this.labelFps.Location = new System.Drawing.Point( 574, 376 );
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size( 27, 13 );
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Location = new System.Drawing.Point( 322, 376 );
      this.checkSmooth.Name = "checkSmooth";
      this.checkSmooth.Size = new System.Drawing.Size( 102, 17 );
      this.checkSmooth.TabIndex = 21;
      this.checkSmooth.Text = "Smooth shading";
      this.checkSmooth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkSmooth.UseVisualStyleBackColor = true;
      // 
      // checkWireframe
      // 
      this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkWireframe.AutoSize = true;
      this.checkWireframe.Location = new System.Drawing.Point( 433, 376 );
      this.checkWireframe.Name = "checkWireframe";
      this.checkWireframe.Size = new System.Drawing.Size( 74, 17 );
      this.checkWireframe.TabIndex = 22;
      this.checkWireframe.Text = "Wireframe";
      this.checkWireframe.UseVisualStyleBackColor = true;
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point( 146, 374 );
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size( 148, 20 );
      this.textParam.TabIndex = 23;
      this.textParam.Text = "200";
      // 
      // checkCamera
      // 
      this.checkCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkCamera.AutoSize = true;
      this.checkCamera.Checked = true;
      this.checkCamera.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkCamera.Location = new System.Drawing.Point( 322, 401 );
      this.checkCamera.Name = "checkCamera";
      this.checkCamera.Size = new System.Drawing.Size( 114, 17 );
      this.checkCamera.TabIndex = 24;
      this.checkCamera.Text = "Camera movement";
      this.checkCamera.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkCamera.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 439 );
      this.Controls.Add( this.checkCamera );
      this.Controls.Add( this.textParam );
      this.Controls.Add( this.checkWireframe );
      this.Controls.Add( this.checkSmooth );
      this.Controls.Add( this.labelFps );
      this.Controls.Add( this.glControl1 );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.buttonInit );
      this.MinimumSize = new System.Drawing.Size( 660, 200 );
      this.Name = "Form1";
      this.Text = "058 marbles";
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonInit;
    private System.Windows.Forms.Label label3;
    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.CheckBox checkSmooth;
    private System.Windows.Forms.CheckBox checkWireframe;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.CheckBox checkCamera;
  }
}

