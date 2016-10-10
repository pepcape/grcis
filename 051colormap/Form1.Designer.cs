namespace _051colormap
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
      this.buttonClose = new System.Windows.Forms.Button();
      this.numColors = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.buttonLoad = new System.Windows.Forms.Button();
      this.pictureInput = new System.Windows.Forms.PictureBox();
      this.pictureBox1 = new _051colormap.MyBox();
      ((System.ComponentModel.ISupportInitialize)(this.numColors)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonClose
      // 
      this.buttonClose.Location = new System.Drawing.Point(671, 338);
      this.buttonClose.Name = "buttonClose";
      this.buttonClose.Size = new System.Drawing.Size(111, 24);
      this.buttonClose.TabIndex = 7;
      this.buttonClose.Text = "Done";
      this.buttonClose.UseVisualStyleBackColor = true;
      this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
      // 
      // numColors
      // 
      this.numColors.Location = new System.Drawing.Point(439, 340);
      this.numColors.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numColors.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
      this.numColors.Name = "numColors";
      this.numColors.Size = new System.Drawing.Size(46, 20);
      this.numColors.TabIndex = 8;
      this.numColors.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
      this.numColors.ValueChanged += new System.EventHandler(this.numColors_ValueChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(369, 342);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(64, 13);
      this.label3.TabIndex = 9;
      this.label3.Text = "Palette size:";
      // 
      // buttonLoad
      // 
      this.buttonLoad.Location = new System.Drawing.Point(533, 338);
      this.buttonLoad.Name = "buttonLoad";
      this.buttonLoad.Size = new System.Drawing.Size(113, 23);
      this.buttonLoad.TabIndex = 10;
      this.buttonLoad.Text = "Load image";
      this.buttonLoad.UseVisualStyleBackColor = true;
      this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
      // 
      // pictureInput
      // 
      this.pictureInput.Location = new System.Drawing.Point(372, 13);
      this.pictureInput.Name = "pictureInput";
      this.pictureInput.Size = new System.Drawing.Size(410, 310);
      this.pictureInput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureInput.TabIndex = 11;
      this.pictureInput.TabStop = false;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(12, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(341, 348);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(794, 372);
      this.Controls.Add(this.pictureInput);
      this.Controls.Add(this.buttonLoad);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.numColors);
      this.Controls.Add(this.buttonClose);
      this.Controls.Add(this.pictureBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "Form1";
      this.Text = "051 Colormap";
      ((System.ComponentModel.ISupportInitialize)(this.numColors)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    public MyBox pictureBox1;
    public System.Windows.Forms.Button buttonClose;
    public System.Windows.Forms.NumericUpDown numColors;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button buttonLoad;
    private System.Windows.Forms.PictureBox pictureInput;
  }
}
