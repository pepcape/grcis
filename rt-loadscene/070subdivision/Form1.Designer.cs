namespace _070subdivision
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
      this.numericSubdivision = new System.Windows.Forms.NumericUpDown();
      this.buttonOpen = new System.Windows.Forms.Button();
      this.labelFile = new System.Windows.Forms.Label();
      this.textParam = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.checkAnimate = new System.Windows.Forms.CheckBox();
      this.checkNormals = new System.Windows.Forms.CheckBox();
      this.checkColors = new System.Windows.Forms.CheckBox();
      this.buttonDivide = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.numericSubdivision)).BeginInit();
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
      this.glControl1.Size = new System.Drawing.Size(680, 350);
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
      this.labelFps.Location = new System.Drawing.Point(587, 379);
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size(27, 13);
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonReset.Location = new System.Drawing.Point(590, 411);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(103, 23);
      this.buttonReset.TabIndex = 22;
      this.buttonReset.Text = "Reset view";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      // 
      // labelSensitivity
      // 
      this.labelSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelSensitivity.AutoSize = true;
      this.labelSensitivity.Location = new System.Drawing.Point(320, 379);
      this.labelSensitivity.Name = "labelSensitivity";
      this.labelSensitivity.Size = new System.Drawing.Size(61, 13);
      this.labelSensitivity.TabIndex = 23;
      this.labelSensitivity.Text = "Subdivision";
      // 
      // numericSubdivision
      // 
      this.numericSubdivision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSubdivision.DecimalPlaces = 3;
      this.numericSubdivision.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
      this.numericSubdivision.Location = new System.Drawing.Point(387, 377);
      this.numericSubdivision.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numericSubdivision.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
      this.numericSubdivision.Name = "numericSubdivision";
      this.numericSubdivision.Size = new System.Drawing.Size(61, 20);
      this.numericSubdivision.TabIndex = 24;
      this.numericSubdivision.Value = new decimal(new int[] {
            8,
            0,
            0,
            196608});
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(15, 375);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(108, 23);
      this.buttonOpen.TabIndex = 25;
      this.buttonOpen.Text = "Load mesh";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // labelFile
      // 
      this.labelFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelFile.AutoSize = true;
      this.labelFile.Location = new System.Drawing.Point(135, 380);
      this.labelFile.Name = "labelFile";
      this.labelFile.Size = new System.Drawing.Size(88, 13);
      this.labelFile.TabIndex = 26;
      this.labelFile.Text = "-- no file loaded --";
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point(180, 414);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(127, 20);
      this.textParam.TabIndex = 29;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(135, 417);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 28;
      this.label3.Text = "Param:";
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point(15, 411);
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size(108, 23);
      this.buttonGenerate.TabIndex = 27;
      this.buttonGenerate.Text = "Generate mesh";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
      // 
      // checkAnimate
      // 
      this.checkAnimate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAnimate.AutoSize = true;
      this.checkAnimate.Location = new System.Drawing.Point(326, 416);
      this.checkAnimate.Name = "checkAnimate";
      this.checkAnimate.Size = new System.Drawing.Size(64, 17);
      this.checkAnimate.TabIndex = 30;
      this.checkAnimate.Text = "Animate";
      this.checkAnimate.UseVisualStyleBackColor = true;
      // 
      // checkNormals
      // 
      this.checkNormals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkNormals.AutoSize = true;
      this.checkNormals.Location = new System.Drawing.Point(396, 416);
      this.checkNormals.Name = "checkNormals";
      this.checkNormals.Size = new System.Drawing.Size(64, 17);
      this.checkNormals.TabIndex = 31;
      this.checkNormals.Text = "Normals";
      this.checkNormals.UseVisualStyleBackColor = true;
      // 
      // checkColors
      // 
      this.checkColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkColors.AutoSize = true;
      this.checkColors.Location = new System.Drawing.Point(466, 416);
      this.checkColors.Name = "checkColors";
      this.checkColors.Size = new System.Drawing.Size(55, 17);
      this.checkColors.TabIndex = 32;
      this.checkColors.Text = "Colors";
      this.checkColors.UseVisualStyleBackColor = true;
      // 
      // buttonDivide
      // 
      this.buttonDivide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonDivide.Location = new System.Drawing.Point(463, 375);
      this.buttonDivide.Name = "buttonDivide";
      this.buttonDivide.Size = new System.Drawing.Size(55, 23);
      this.buttonDivide.TabIndex = 33;
      this.buttonDivide.Text = "Divide";
      this.buttonDivide.UseVisualStyleBackColor = true;
      this.buttonDivide.Click += new System.EventHandler(this.buttonDivide_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(712, 446);
      this.Controls.Add(this.buttonDivide);
      this.Controls.Add(this.checkColors);
      this.Controls.Add(this.checkNormals);
      this.Controls.Add(this.checkAnimate);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.buttonGenerate);
      this.Controls.Add(this.labelFile);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.numericSubdivision);
      this.Controls.Add(this.labelSensitivity);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.labelFps);
      this.Controls.Add(this.glControl1);
      this.MinimumSize = new System.Drawing.Size(670, 200);
      this.Name = "Form1";
      this.Text = "070 subdivision";
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
      ((System.ComponentModel.ISupportInitialize)(this.numericSubdivision)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.Label labelSensitivity;
    private System.Windows.Forms.NumericUpDown numericSubdivision;
    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Label labelFile;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.CheckBox checkAnimate;
    private System.Windows.Forms.CheckBox checkNormals;
    private System.Windows.Forms.CheckBox checkColors;
    private System.Windows.Forms.Button buttonDivide;
  }
}

