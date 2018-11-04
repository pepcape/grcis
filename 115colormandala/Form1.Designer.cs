namespace _115colormandala
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
      this.numColors = new System.Windows.Forms.NumericUpDown();
      this.buttonRecompute = new System.Windows.Forms.Button();
      this.textParam = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.pictureBox1 = new _115colormandala.MyBox();
      ((System.ComponentModel.ISupportInitialize)(this.numColors)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // numColors
      // 
      this.numColors.Location = new System.Drawing.Point(340, 508);
      this.numColors.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
      this.numColors.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numColors.Name = "numColors";
      this.numColors.Size = new System.Drawing.Size(46, 20);
      this.numColors.TabIndex = 3;
      this.numColors.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numColors.ValueChanged += new System.EventHandler(this.numColors_ValueChanged);
      // 
      // buttonRecompute
      // 
      this.buttonRecompute.Location = new System.Drawing.Point(401, 506);
      this.buttonRecompute.Name = "buttonRecompute";
      this.buttonRecompute.Size = new System.Drawing.Size(90, 23);
      this.buttonRecompute.TabIndex = 4;
      this.buttonRecompute.Text = "Recompute";
      this.buttonRecompute.UseVisualStyleBackColor = true;
      this.buttonRecompute.Click += new System.EventHandler(this.buttonRecompute_Click);
      this.buttonRecompute.MouseHover += new System.EventHandler(this.buttonRecompute_MouseHover);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point(53, 507);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(235, 20);
      this.textParam.TabIndex = 1;
      this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
      this.textParam.MouseHover += new System.EventHandler(this.textParam_MouseHover);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(10, 510);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(40, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Param:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(297, 510);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(39, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Colors:";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(12, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(480, 480);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(505, 540);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.buttonRecompute);
      this.Controls.Add(this.numColors);
      this.Controls.Add(this.pictureBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "Form1";
      this.Text = "115 Colormap mandala";
      ((System.ComponentModel.ISupportInitialize)(this.numColors)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    public MyBox pictureBox1;
    public System.Windows.Forms.NumericUpDown numColors;
    private System.Windows.Forms.Button buttonRecompute;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
  }
}
