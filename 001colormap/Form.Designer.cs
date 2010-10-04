namespace _001colormap
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
    protected override void Dispose( bool disposing )
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
    private void InitializeComponent()
    {
      this.button1 = new System.Windows.Forms.Button();
      this.colorDialog1 = new System.Windows.Forms.ColorDialog();
      this.buttonColor1 = new System.Windows.Forms.Button();
      this.buttonColor2 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.numColors = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.baseBox2 = new _001colormap.BaseBox();
      this.baseBox1 = new _001colormap.BaseBox();
      this.pictureBox1 = new _001colormap.MyBox();
      ((System.ComponentModel.ISupportInitialize)(this.numColors)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.baseBox2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.baseBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point( 433, 329 );
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size( 249, 31 );
      this.button1.TabIndex = 7;
      this.button1.Text = "Done";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler( this.button1_Click );
      // 
      // buttonColor1
      // 
      this.buttonColor1.Location = new System.Drawing.Point( 433, 13 );
      this.buttonColor1.Name = "buttonColor1";
      this.buttonColor1.Size = new System.Drawing.Size( 249, 23 );
      this.buttonColor1.TabIndex = 1;
      this.buttonColor1.Text = "Select base color 1";
      this.buttonColor1.UseVisualStyleBackColor = true;
      this.buttonColor1.Click += new System.EventHandler( this.buttonColor1_Click );
      // 
      // buttonColor2
      // 
      this.buttonColor2.Location = new System.Drawing.Point( 433, 137 );
      this.buttonColor2.Name = "buttonColor2";
      this.buttonColor2.Size = new System.Drawing.Size( 249, 23 );
      this.buttonColor2.TabIndex = 4;
      this.buttonColor2.Text = "Select base color 2";
      this.buttonColor2.UseVisualStyleBackColor = true;
      this.buttonColor2.Click += new System.EventHandler( this.buttonColor2_Click );
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point( 581, 73 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 80, 13 );
      this.label1.TabIndex = 3;
      this.label1.Text = "--";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point( 581, 196 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 80, 13 );
      this.label2.TabIndex = 6;
      this.label2.Text = "--";
      // 
      // numColors
      // 
      this.numColors.Location = new System.Drawing.Point( 516, 265 );
      this.numColors.Maximum = new decimal( new int[] {
            9,
            0,
            0,
            0} );
      this.numColors.Minimum = new decimal( new int[] {
            3,
            0,
            0,
            0} );
      this.numColors.Name = "numColors";
      this.numColors.Size = new System.Drawing.Size( 74, 20 );
      this.numColors.TabIndex = 8;
      this.numColors.Value = new decimal( new int[] {
            4,
            0,
            0,
            0} );
      this.numColors.ValueChanged += new System.EventHandler( this.numColors_ValueChanged );
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 433, 266 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 64, 13 );
      this.label3.TabIndex = 9;
      this.label3.Text = "Palette size:";
      // 
      // baseBox2
      // 
      this.baseBox2.color = System.Drawing.Color.Empty;
      this.baseBox2.Location = new System.Drawing.Point( 464, 179 );
      this.baseBox2.Name = "baseBox2";
      this.baseBox2.Size = new System.Drawing.Size( 100, 50 );
      this.baseBox2.TabIndex = 5;
      this.baseBox2.TabStop = false;
      // 
      // baseBox1
      // 
      this.baseBox1.color = System.Drawing.Color.Empty;
      this.baseBox1.Location = new System.Drawing.Point( 464, 54 );
      this.baseBox1.Name = "baseBox1";
      this.baseBox1.Size = new System.Drawing.Size( 100, 50 );
      this.baseBox1.TabIndex = 2;
      this.baseBox1.TabStop = false;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point( 12, 12 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 400, 348 );
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 694, 372 );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.numColors );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.baseBox2 );
      this.Controls.Add( this.buttonColor2 );
      this.Controls.Add( this.baseBox1 );
      this.Controls.Add( this.buttonColor1 );
      this.Controls.Add( this.button1 );
      this.Controls.Add( this.pictureBox1 );
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "Form1";
      this.Text = "001 Paleta";
      ((System.ComponentModel.ISupportInitialize)(this.numColors)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.baseBox2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.baseBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    public MyBox pictureBox1;
    public System.Windows.Forms.Button button1;
    public System.Windows.Forms.Button buttonColor1;
    public System.Windows.Forms.ColorDialog colorDialog1;
    public BaseBox baseBox1;
    public System.Windows.Forms.Button buttonColor2;
    public BaseBox baseBox2;
    public System.Windows.Forms.Label label1;
    public System.Windows.Forms.Label label2;
    public System.Windows.Forms.NumericUpDown numColors;
    private System.Windows.Forms.Label label3;
  }
}
