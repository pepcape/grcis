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
      this.panel1 = new System.Windows.Forms.Panel();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.buttonInput = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.numericFactor = new System.Windows.Forms.NumericUpDown();
      this.comboFunction = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFactor)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.AutoScroll = true;
      this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add( this.pictureBox1 );
      this.panel1.Location = new System.Drawing.Point( 13, 13 );
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size( 680, 380 );
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point( 0, 0 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 680, 380 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      // 
      // buttonInput
      // 
      this.buttonInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonInput.Location = new System.Drawing.Point( 13, 410 );
      this.buttonInput.Name = "buttonInput";
      this.buttonInput.Size = new System.Drawing.Size( 122, 23 );
      this.buttonInput.TabIndex = 1;
      this.buttonInput.Text = "Input image";
      this.buttonInput.UseVisualStyleBackColor = true;
      this.buttonInput.Click += new System.EventHandler( this.buttonInput_Click );
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point( 599, 409 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 92, 23 );
      this.buttonSave.TabIndex = 3;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 339, 415 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 40, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Factor:";
      // 
      // numericFactor
      // 
      this.numericFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericFactor.DecimalPlaces = 2;
      this.numericFactor.Increment = new decimal( new int[] {
            1,
            0,
            0,
            65536} );
      this.numericFactor.Location = new System.Drawing.Point( 385, 412 );
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
      this.numericFactor.Size = new System.Drawing.Size( 67, 20 );
      this.numericFactor.TabIndex = 5;
      this.numericFactor.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      // 
      // comboFunction
      // 
      this.comboFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboFunction.FormattingEnabled = true;
      this.comboFunction.Location = new System.Drawing.Point( 218, 412 );
      this.comboFunction.Name = "comboFunction";
      this.comboFunction.Size = new System.Drawing.Size( 100, 21 );
      this.comboFunction.TabIndex = 24;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 153, 416 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 51, 13 );
      this.label2.TabIndex = 25;
      this.label2.Text = "Function:";
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRedraw.Location = new System.Drawing.Point( 481, 409 );
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size( 89, 23 );
      this.buttonRedraw.TabIndex = 26;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler( this.buttonRedraw_Click );
      // 
      // FormWarping
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 446 );
      this.Controls.Add( this.buttonRedraw );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.comboFunction );
      this.Controls.Add( this.numericFactor );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.buttonInput );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 710, 200 );
      this.Name = "FormWarping";
      this.Text = "002 warping";
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFactor)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonInput;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.NumericUpDown numericFactor;
    private System.Windows.Forms.ComboBox comboFunction;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button buttonRedraw;
  }
}
