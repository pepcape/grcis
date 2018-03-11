namespace _006warping
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
      this.pictureResult = new GUIPictureBox();
      this.buttonOpen = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.numericParam = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureResult)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureResult
      // 
      this.pictureResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureResult.Location = new System.Drawing.Point( 13, 13 );
      this.pictureResult.Name = "pictureResult";
      this.pictureResult.Size = new System.Drawing.Size( 680, 380 );
      this.pictureResult.TabIndex = 0;
      this.pictureResult.TabStop = false;
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point( 12, 411 );
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size( 130, 23 );
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load image";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler( this.buttonOpen_Click );
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point( 570, 411 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 130, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // numericParam
      // 
      this.numericParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericParam.DecimalPlaces = 2;
      this.numericParam.Increment = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.numericParam.Location = new System.Drawing.Point( 241, 414 );
      this.numericParam.Name = "numericParam";
      this.numericParam.Size = new System.Drawing.Size( 79, 20 );
      this.numericParam.TabIndex = 3;
      this.numericParam.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.numericParam.ValueChanged += new System.EventHandler( this.numericParam_ValueChanged );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.Location = new System.Drawing.Point( 168, 416 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 58, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Parameter:";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericParam );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonOpen );
      this.Controls.Add( this.pictureResult );
      this.MinimumSize = new System.Drawing.Size( 620, 200 );
      this.Name = "Form1";
      this.Text = "006 drag warping";
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureResult)).EndInit();
      this.ResumeLayout( false );
    }

    #endregion

    public GUIPictureBox pictureResult;
    public System.Windows.Forms.Button buttonOpen;
    public System.Windows.Forms.Button buttonSave;
    public System.Windows.Forms.NumericUpDown numericParam;
    private System.Windows.Forms.Label label1;
  }
}
