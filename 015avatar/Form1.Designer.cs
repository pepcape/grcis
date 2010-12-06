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
      ((System.ComponentModel.ISupportInitialize)(this.numericVariant)).BeginInit();
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
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl1_Paint );
      this.glControl1.Resize += new System.EventHandler( this.glControl1_Resize );
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
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
  }
}

