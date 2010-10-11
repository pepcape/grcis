namespace _003colortransform
{
  partial class FormColorTransform
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
      this.pictureResult = new System.Windows.Forms.PictureBox();
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
      this.pictureResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureResult.Location = new System.Drawing.Point( 13, 13 );
      this.pictureResult.Name = "pictureResult";
      this.pictureResult.Size = new System.Drawing.Size( 714, 366 );
      this.pictureResult.TabIndex = 0;
      this.pictureResult.TabStop = false;
      // 
      // buttonOpen
      // 
      this.buttonOpen.Location = new System.Drawing.Point( 13, 392 );
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size( 172, 23 );
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load image";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler( this.buttonOpen_Click );
      // 
      // buttonSave
      // 
      this.buttonSave.Location = new System.Drawing.Point( 534, 392 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 192, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // numericParam
      // 
      this.numericParam.DecimalPlaces = 2;
      this.numericParam.Increment = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.numericParam.Location = new System.Drawing.Point( 336, 392 );
      this.numericParam.Name = "numericParam";
      this.numericParam.Size = new System.Drawing.Size( 131, 20 );
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
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 263, 394 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 58, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Parameter:";
      // 
      // FormColorTransform
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.ClientSize = new System.Drawing.Size( 748, 427 );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericParam );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonOpen );
      this.Controls.Add( this.pictureResult );
      this.Name = "FormColorTransform";
      this.Text = "003 color transform";
      ((System.ComponentModel.ISupportInitialize)(this.pictureResult)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    public System.Windows.Forms.PictureBox pictureResult;
    public System.Windows.Forms.Button buttonOpen;
    public System.Windows.Forms.Button buttonSave;
    public System.Windows.Forms.NumericUpDown numericParam;
    private System.Windows.Forms.Label label1;
  }
}

