namespace _002warping
{
  partial class FormWarping
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
      this.outputPictureBox = new System.Windows.Forms.PictureBox();
      this.buttonInput = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.numericFactor = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFactor)).BeginInit();
      this.SuspendLayout();
      // 
      // outputPictureBox
      // 
      this.outputPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.outputPictureBox.Location = new System.Drawing.Point( 13, 13 );
      this.outputPictureBox.Name = "outputPictureBox";
      this.outputPictureBox.Size = new System.Drawing.Size( 628, 406 );
      this.outputPictureBox.TabIndex = 0;
      this.outputPictureBox.TabStop = false;
      // 
      // buttonInput
      // 
      this.buttonInput.Location = new System.Drawing.Point( 13, 437 );
      this.buttonInput.Name = "buttonInput";
      this.buttonInput.Size = new System.Drawing.Size( 173, 23 );
      this.buttonInput.TabIndex = 1;
      this.buttonInput.Text = "Input image";
      this.buttonInput.UseVisualStyleBackColor = true;
      this.buttonInput.Click += new System.EventHandler( this.buttonInput_Click );
      // 
      // buttonSave
      // 
      this.buttonSave.Location = new System.Drawing.Point( 459, 436 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 181, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 235, 442 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 66, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Warp factor:";
      // 
      // numericFactor
      // 
      this.numericFactor.DecimalPlaces = 2;
      this.numericFactor.Increment = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.numericFactor.Location = new System.Drawing.Point( 317, 439 );
      this.numericFactor.Maximum = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.numericFactor.Minimum = new decimal( new int[] {
            5,
            0,
            0,
            131072} );
      this.numericFactor.Name = "numericFactor";
      this.numericFactor.Size = new System.Drawing.Size( 78, 20 );
      this.numericFactor.TabIndex = 5;
      this.numericFactor.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.numericFactor.ValueChanged += new System.EventHandler( this.numericFactor_ValueChanged );
      // 
      // FormWarping
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.ClientSize = new System.Drawing.Size( 653, 474 );
      this.Controls.Add( this.numericFactor );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonInput );
      this.Controls.Add( this.outputPictureBox );
      this.Name = "FormWarping";
      this.Text = "002 warping";
      ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFactor)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    public System.Windows.Forms.PictureBox outputPictureBox;
    public System.Windows.Forms.Button buttonInput;
    public System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Label label1;
    public System.Windows.Forms.NumericUpDown numericFactor;
  }
}
