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
      this.upDownAzimuth = new System.Windows.Forms.NumericUpDown();
      this.upDownElevation = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.upDownZoom = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.buttonRegenerate = new System.Windows.Forms.Button();
      this.upDownRoughness = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.upDownIterations)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownAzimuth)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownElevation)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownZoom)).BeginInit();
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
      this.glControl1.Location = new System.Drawing.Point( 13, 12 );
      this.glControl1.Name = "glControl1";
      this.glControl1.Size = new System.Drawing.Size( 769, 434 );
      this.glControl1.TabIndex = 17;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler( this.glControl1_Load );
      this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseWheel );
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl1_Paint );
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseMove );
      this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler( this.glControl1_KeyUp );
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseDown );
      this.glControl1.Resize += new System.EventHandler( this.glControl1_Resize );
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseUp );
      this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler( this.glControl1_KeyDown );
      // 
      // labelFps
      // 
      this.labelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelFps.AutoSize = true;
      this.labelFps.Location = new System.Drawing.Point( 653, 467 );
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size( 27, 13 );
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Enabled = false;
      this.buttonSave.Location = new System.Drawing.Point( 656, 522 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 126, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // upDownIterations
      // 
      this.upDownIterations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.upDownIterations.Location = new System.Drawing.Point( 193, 488 );
      this.upDownIterations.Maximum = new decimal( new int[] {
            1000,
            0,
            0,
            0} );
      this.upDownIterations.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.upDownIterations.Name = "upDownIterations";
      this.upDownIterations.Size = new System.Drawing.Size( 66, 20 );
      this.upDownIterations.TabIndex = 19;
      this.upDownIterations.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      // 
      // upDownAzimuth
      // 
      this.upDownAzimuth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.upDownAzimuth.DecimalPlaces = 4;
      this.upDownAzimuth.Increment = new decimal( new int[] {
            5,
            0,
            0,
            131072} );
      this.upDownAzimuth.Location = new System.Drawing.Point( 400, 488 );
      this.upDownAzimuth.Minimum = new decimal( new int[] {
            100,
            0,
            0,
            -2147483648} );
      this.upDownAzimuth.Name = "upDownAzimuth";
      this.upDownAzimuth.Size = new System.Drawing.Size( 63, 20 );
      this.upDownAzimuth.TabIndex = 20;
      this.upDownAzimuth.ValueChanged += new System.EventHandler( this.upDownAzimuth_ValueChanged );
      // 
      // upDownElevation
      // 
      this.upDownElevation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.upDownElevation.DecimalPlaces = 4;
      this.upDownElevation.Increment = new decimal( new int[] {
            5,
            0,
            0,
            131072} );
      this.upDownElevation.Location = new System.Drawing.Point( 400, 515 );
      this.upDownElevation.Maximum = new decimal( new int[] {
            15,
            0,
            0,
            65536} );
      this.upDownElevation.Name = "upDownElevation";
      this.upDownElevation.Size = new System.Drawing.Size( 63, 20 );
      this.upDownElevation.TabIndex = 21;
      this.upDownElevation.ValueChanged += new System.EventHandler( this.upDownElevation_ValueChanged );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 351, 491 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 43, 13 );
      this.label1.TabIndex = 22;
      this.label1.Text = "aziumth";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 344, 517 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 50, 13 );
      this.label2.TabIndex = 23;
      this.label2.Text = "elevation";
      // 
      // upDownZoom
      // 
      this.upDownZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.upDownZoom.DecimalPlaces = 1;
      this.upDownZoom.Increment = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.upDownZoom.Location = new System.Drawing.Point( 400, 462 );
      this.upDownZoom.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.upDownZoom.Name = "upDownZoom";
      this.upDownZoom.Size = new System.Drawing.Size( 63, 20 );
      this.upDownZoom.TabIndex = 24;
      this.upDownZoom.Value = new decimal( new int[] {
            3,
            0,
            0,
            0} );
      this.upDownZoom.ValueChanged += new System.EventHandler( this.upDownZoom_ValueChanged );
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 362, 464 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 32, 13 );
      this.label3.TabIndex = 25;
      this.label3.Text = "zoom";
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point( 138, 491 );
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size( 49, 13 );
      this.label4.TabIndex = 27;
      this.label4.Text = "iterations";
      // 
      // label5
      // 
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point( 70, 517 );
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size( 117, 13 );
      this.label5.TabIndex = 28;
      this.label5.Text = "roughness/smoothness";
      // 
      // buttonRegenerate
      // 
      this.buttonRegenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRegenerate.Location = new System.Drawing.Point( 656, 491 );
      this.buttonRegenerate.Name = "buttonRegenerate";
      this.buttonRegenerate.Size = new System.Drawing.Size( 126, 23 );
      this.buttonRegenerate.TabIndex = 33;
      this.buttonRegenerate.Text = "Regenerate terrain";
      this.buttonRegenerate.UseVisualStyleBackColor = true;
      this.buttonRegenerate.Click += new System.EventHandler( this.buttonRegenerate_Click );
      // 
      // upDownRoughness
      // 
      this.upDownRoughness.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.upDownRoughness.DecimalPlaces = 2;
      this.upDownRoughness.Increment = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.upDownRoughness.Location = new System.Drawing.Point( 193, 514 );
      this.upDownRoughness.Maximum = new decimal( new int[] {
            5,
            0,
            0,
            0} );
      this.upDownRoughness.Name = "upDownRoughness";
      this.upDownRoughness.Size = new System.Drawing.Size( 65, 20 );
      this.upDownRoughness.TabIndex = 26;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 801, 556 );
      this.Controls.Add( this.buttonRegenerate );
      this.Controls.Add( this.label5 );
      this.Controls.Add( this.label4 );
      this.Controls.Add( this.upDownRoughness );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.upDownZoom );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.upDownElevation );
      this.Controls.Add( this.upDownAzimuth );
      this.Controls.Add( this.upDownIterations );
      this.Controls.Add( this.labelFps );
      this.Controls.Add( this.glControl1 );
      this.Controls.Add( this.buttonSave );
      this.MinimumSize = new System.Drawing.Size( 660, 200 );
      this.Name = "Form1";
      this.Text = "039 terrain";
      ((System.ComponentModel.ISupportInitialize)(this.upDownIterations)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownAzimuth)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownElevation)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownZoom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.upDownRoughness)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown upDownIterations;
    private System.Windows.Forms.NumericUpDown upDownAzimuth;
    private System.Windows.Forms.NumericUpDown upDownElevation;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown upDownZoom;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button buttonRegenerate;
    private System.Windows.Forms.NumericUpDown upDownRoughness;
  }
}

