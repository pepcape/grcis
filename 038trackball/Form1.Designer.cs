namespace _038trackball
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
      this.labelSensitivity = new System.Windows.Forms.Label();
      this.numericSensitivity = new System.Windows.Forms.NumericUpDown();
      this.buttonOpen = new System.Windows.Forms.Button();
      this.labelFile = new System.Windows.Forms.Label();
      this.textParam = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.checkMulti = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).BeginInit();
      this.SuspendLayout();
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
      this.glControl1.Size = new System.Drawing.Size( 680, 350 );
      this.glControl1.TabIndex = 17;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler( this.glControl1_Load );
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl1_Paint );
      this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler( this.glControl1_KeyDown );
      this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler( this.glControl1_KeyUp );
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseDown );
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseMove );
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseUp );
      this.glControl1.Resize += new System.EventHandler( this.glControl1_Resize );
      // 
      // labelFps
      // 
      this.labelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelFps.AutoSize = true;
      this.labelFps.Location = new System.Drawing.Point( 566, 377 );
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size( 27, 13 );
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonReset.Location = new System.Drawing.Point( 602, 411 );
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size( 91, 23 );
      this.buttonReset.TabIndex = 22;
      this.buttonReset.Text = "Reset";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler( this.buttonReset_Click );
      // 
      // labelSensitivity
      // 
      this.labelSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelSensitivity.AutoSize = true;
      this.labelSensitivity.Location = new System.Drawing.Point( 323, 379 );
      this.labelSensitivity.Name = "labelSensitivity";
      this.labelSensitivity.Size = new System.Drawing.Size( 54, 13 );
      this.labelSensitivity.TabIndex = 23;
      this.labelSensitivity.Text = "Sensitivity";
      // 
      // numericSensitivity
      // 
      this.numericSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSensitivity.DecimalPlaces = 2;
      this.numericSensitivity.Location = new System.Drawing.Point( 383, 377 );
      this.numericSensitivity.Maximum = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.numericSensitivity.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.numericSensitivity.Name = "numericSensitivity";
      this.numericSensitivity.Size = new System.Drawing.Size( 75, 20 );
      this.numericSensitivity.TabIndex = 24;
      this.numericSensitivity.Value = new decimal( new int[] {
            10,
            0,
            0,
            65536} );
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point( 15, 375 );
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size( 108, 23 );
      this.buttonOpen.TabIndex = 25;
      this.buttonOpen.Text = "Load scene";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler( this.buttonOpen_Click );
      // 
      // labelFile
      // 
      this.labelFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelFile.AutoSize = true;
      this.labelFile.Location = new System.Drawing.Point( 135, 380 );
      this.labelFile.Name = "labelFile";
      this.labelFile.Size = new System.Drawing.Size( 88, 13 );
      this.labelFile.TabIndex = 26;
      this.labelFile.Text = "-- no file loaded --";
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point( 180, 414 );
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size( 121, 20 );
      this.textParam.TabIndex = 29;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 135, 417 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 40, 13 );
      this.label3.TabIndex = 28;
      this.label3.Text = "Param:";
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point( 15, 411 );
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size( 108, 23 );
      this.buttonGenerate.TabIndex = 27;
      this.buttonGenerate.Text = "Generate";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler( this.buttonGenerate_Click );
      // 
      // checkMulti
      // 
      this.checkMulti.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkMulti.AutoSize = true;
      this.checkMulti.Location = new System.Drawing.Point( 328, 416 );
      this.checkMulti.Name = "checkMulti";
      this.checkMulti.Size = new System.Drawing.Size( 86, 17 );
      this.checkMulti.TabIndex = 30;
      this.checkMulti.Text = "8 instances?";
      this.checkMulti.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.checkMulti );
      this.Controls.Add( this.textParam );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.buttonGenerate );
      this.Controls.Add( this.labelFile );
      this.Controls.Add( this.buttonOpen );
      this.Controls.Add( this.numericSensitivity );
      this.Controls.Add( this.labelSensitivity );
      this.Controls.Add( this.buttonReset );
      this.Controls.Add( this.labelFps );
      this.Controls.Add( this.glControl1 );
      this.MinimumSize = new System.Drawing.Size( 660, 200 );
      this.Name = "Form1";
      this.Text = "038 trackball";
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseWheel );
      ((System.ComponentModel.ISupportInitialize)(this.numericSensitivity)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.Label labelSensitivity;
    private System.Windows.Forms.NumericUpDown numericSensitivity;
    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Label labelFile;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.CheckBox checkMulti;
  }
}

