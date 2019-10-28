namespace Modules
{
  partial class FormHSV
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.buttonRecompute = new System.Windows.Forms.Button();
      this.buttonDeactivate = new System.Windows.Forms.Button();
      this.buttonReset = new System.Windows.Forms.Button();
      this.numericHue = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textSaturation = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.textValue = new System.Windows.Forms.TextBox();
      this.checkParallel = new System.Windows.Forms.CheckBox();
      this.checkSlow = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.textGamma = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.numericHue)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonRecompute
      // 
      this.buttonRecompute.Location = new System.Drawing.Point(17, 174);
      this.buttonRecompute.Name = "buttonRecompute";
      this.buttonRecompute.Size = new System.Drawing.Size(147, 23);
      this.buttonRecompute.TabIndex = 11;
      this.buttonRecompute.Text = "Recompute";
      this.buttonRecompute.UseVisualStyleBackColor = true;
      this.buttonRecompute.Click += new System.EventHandler(this.buttonRecompute_Click);
      // 
      // buttonDeactivate
      // 
      this.buttonDeactivate.Location = new System.Drawing.Point(179, 174);
      this.buttonDeactivate.Name = "buttonDeactivate";
      this.buttonDeactivate.Size = new System.Drawing.Size(109, 23);
      this.buttonDeactivate.TabIndex = 12;
      this.buttonDeactivate.Text = "Deactivate module";
      this.buttonDeactivate.UseVisualStyleBackColor = true;
      this.buttonDeactivate.Click += new System.EventHandler(this.buttonDeactivate_Click);
      // 
      // buttonReset
      // 
      this.buttonReset.Location = new System.Drawing.Point(201, 17);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(87, 23);
      this.buttonReset.TabIndex = 2;
      this.buttonReset.Text = "Reset values";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      // 
      // numericHue
      // 
      this.numericHue.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericHue.Location = new System.Drawing.Point(89, 19);
      this.numericHue.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
      this.numericHue.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
      this.numericHue.Name = "numericHue";
      this.numericHue.Size = new System.Drawing.Size(87, 20);
      this.numericHue.TabIndex = 1;
      this.numericHue.ValueChanged += new System.EventHandler(this.numericHue_ValueChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(18, 21);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(27, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Hue";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(18, 60);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Saturation";
      // 
      // textSaturation
      // 
      this.textSaturation.CausesValidation = false;
      this.textSaturation.Location = new System.Drawing.Point(89, 58);
      this.textSaturation.Name = "textSaturation";
      this.textSaturation.Size = new System.Drawing.Size(87, 20);
      this.textSaturation.TabIndex = 4;
      this.textSaturation.Text = "1.0";
      this.textSaturation.TextChanged += new System.EventHandler(this.textSaturation_TextChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(21, 99);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(34, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Value";
      // 
      // textValue
      // 
      this.textValue.CausesValidation = false;
      this.textValue.Location = new System.Drawing.Point(89, 97);
      this.textValue.Name = "textValue";
      this.textValue.Size = new System.Drawing.Size(87, 20);
      this.textValue.TabIndex = 7;
      this.textValue.Text = "1.0";
      this.textValue.TextChanged += new System.EventHandler(this.textValue_TextChanged);
      // 
      // checkParallel
      // 
      this.checkParallel.AutoSize = true;
      this.checkParallel.Checked = true;
      this.checkParallel.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkParallel.Location = new System.Drawing.Point(229, 61);
      this.checkParallel.Name = "checkParallel";
      this.checkParallel.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.checkParallel.Size = new System.Drawing.Size(59, 17);
      this.checkParallel.TabIndex = 5;
      this.checkParallel.Text = "parallel";
      this.checkParallel.UseVisualStyleBackColor = true;
      this.checkParallel.CheckedChanged += new System.EventHandler(this.checkParallel_CheckedChanged);
      // 
      // checkSlow
      // 
      this.checkSlow.AutoSize = true;
      this.checkSlow.Location = new System.Drawing.Point(229, 99);
      this.checkSlow.Name = "checkSlow";
      this.checkSlow.Size = new System.Drawing.Size(47, 17);
      this.checkSlow.TabIndex = 8;
      this.checkSlow.Text = "slow";
      this.checkSlow.UseVisualStyleBackColor = true;
      this.checkSlow.CheckedChanged += new System.EventHandler(this.checkSlow_CheckedChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(21, 138);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(43, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Gamma";
      // 
      // textGamma
      // 
      this.textGamma.CausesValidation = false;
      this.textGamma.Location = new System.Drawing.Point(89, 136);
      this.textGamma.Name = "textGamma";
      this.textGamma.Size = new System.Drawing.Size(87, 20);
      this.textGamma.TabIndex = 10;
      this.textGamma.Text = "1.0";
      this.textGamma.TextChanged += new System.EventHandler(this.textGamma_TextChanged);
      // 
      // FormHSV
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(304, 211);
      this.Controls.Add(this.textGamma);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.checkSlow);
      this.Controls.Add(this.checkParallel);
      this.Controls.Add(this.textValue);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.textSaturation);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.numericHue);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.buttonDeactivate);
      this.Controls.Add(this.buttonRecompute);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormHSV";
      this.Text = "Module HSV";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormHSV_FormClosed);
      ((System.ComponentModel.ISupportInitialize)(this.numericHue)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonRecompute;
    private System.Windows.Forms.Button buttonDeactivate;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    public System.Windows.Forms.TextBox textSaturation;
    public System.Windows.Forms.TextBox textValue;
    public System.Windows.Forms.CheckBox checkParallel;
    public System.Windows.Forms.CheckBox checkSlow;
    public System.Windows.Forms.NumericUpDown numericHue;
    private System.Windows.Forms.Label label4;
    public System.Windows.Forms.TextBox textGamma;
  }
}
