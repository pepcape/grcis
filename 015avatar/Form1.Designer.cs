namespace _015avatar
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
      this.buttonOpen = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.labelFaces = new System.Windows.Forms.Label();
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.numericVariant = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.glControl1 = new OpenTK.GLControl();
      this.labelFps = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.numericInstances = new System.Windows.Forms.NumericUpDown();
      this.checkSmooth = new System.Windows.Forms.CheckBox();
      this.checkWireframe = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.numericVariant)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericInstances)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point( 13, 377 );
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size( 108, 23 );
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load scene";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler( this.buttonOpen_Click );
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Enabled = false;
      this.buttonSave.Location = new System.Drawing.Point( 586, 412 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 107, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // labelFaces
      // 
      this.labelFaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelFaces.AutoSize = true;
      this.labelFaces.Location = new System.Drawing.Point( 133, 383 );
      this.labelFaces.Name = "labelFaces";
      this.labelFaces.Size = new System.Drawing.Size( 33, 13 );
      this.labelFaces.TabIndex = 6;
      this.labelFaces.Text = "faces";
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point( 13, 410 );
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size( 108, 23 );
      this.buttonGenerate.TabIndex = 11;
      this.buttonGenerate.Text = "Generate";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler( this.buttonGenerate_Click );
      // 
      // numericVariant
      // 
      this.numericVariant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericVariant.DecimalPlaces = 2;
      this.numericVariant.Location = new System.Drawing.Point( 182, 413 );
      this.numericVariant.Name = "numericVariant";
      this.numericVariant.Size = new System.Drawing.Size( 73, 20 );
      this.numericVariant.TabIndex = 12;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 133, 416 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 43, 13 );
      this.label3.TabIndex = 13;
      this.label3.Text = "Variant:";
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
      this.labelFps.Location = new System.Drawing.Point( 574, 382 );
      this.labelFps.Name = "labelFps";
      this.labelFps.Size = new System.Drawing.Size( 27, 13 );
      this.labelFps.TabIndex = 18;
      this.labelFps.Text = "Fps:";
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 283, 417 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 56, 13 );
      this.label1.TabIndex = 19;
      this.label1.Text = "Instances:";
      // 
      // numericInstances
      // 
      this.numericInstances.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericInstances.Location = new System.Drawing.Point( 349, 414 );
      this.numericInstances.Maximum = new decimal( new int[] {
            25,
            0,
            0,
            0} );
      this.numericInstances.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.numericInstances.Name = "numericInstances";
      this.numericInstances.Size = new System.Drawing.Size( 57, 20 );
      this.numericInstances.TabIndex = 20;
      this.numericInstances.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Location = new System.Drawing.Point( 322, 381 );
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
      this.checkWireframe.Location = new System.Drawing.Point( 433, 381 );
      this.checkWireframe.Name = "checkWireframe";
      this.checkWireframe.Size = new System.Drawing.Size( 74, 17 );
      this.checkWireframe.TabIndex = 22;
      this.checkWireframe.Text = "Wireframe";
      this.checkWireframe.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.checkWireframe );
      this.Controls.Add( this.checkSmooth );
      this.Controls.Add( this.numericInstances );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.labelFps );
      this.Controls.Add( this.glControl1 );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.numericVariant );
      this.Controls.Add( this.buttonGenerate );
      this.Controls.Add( this.labelFaces );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonOpen );
      this.MinimumSize = new System.Drawing.Size( 660, 200 );
      this.Name = "Form1";
      this.Text = "015 avatar";
      ((System.ComponentModel.ISupportInitialize)(this.numericVariant)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericInstances)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Label labelFaces;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.NumericUpDown numericVariant;
    private System.Windows.Forms.Label label3;
    private OpenTK.GLControl glControl1;
    private System.Windows.Forms.Label labelFps;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.NumericUpDown numericInstances;
    private System.Windows.Forms.CheckBox checkSmooth;
    private System.Windows.Forms.CheckBox checkWireframe;
  }
}

