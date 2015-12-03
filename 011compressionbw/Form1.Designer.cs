namespace _011compressionbw
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
      this.panel1 = new System.Windows.Forms.Panel();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.buttonSave = new System.Windows.Forms.Button();
      this.buttonRecode = new System.Windows.Forms.Button();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.labelResult = new System.Windows.Forms.Label();
      this.buttonLoad = new System.Windows.Forms.Button();
      this.checkDiff = new System.Windows.Forms.CheckBox();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.AutoScroll = true;
      this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(13, 13);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(680, 410);
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(680, 410);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(585, 439);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(108, 23);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // buttonRecode
      // 
      this.buttonRecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRecode.Location = new System.Drawing.Point(140, 439);
      this.buttonRecode.Name = "buttonRecode";
      this.buttonRecode.Size = new System.Drawing.Size(97, 23);
      this.buttonRecode.TabIndex = 5;
      this.buttonRecode.Text = "Recode";
      this.buttonRecode.UseVisualStyleBackColor = true;
      this.buttonRecode.Click += new System.EventHandler(this.buttonRecode_Click);
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point(251, 445);
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size(48, 13);
      this.labelElapsed.TabIndex = 8;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelResult
      // 
      this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelResult.AutoSize = true;
      this.labelResult.Location = new System.Drawing.Point(414, 446);
      this.labelResult.Name = "labelResult";
      this.labelResult.Size = new System.Drawing.Size(32, 13);
      this.labelResult.TabIndex = 9;
      this.labelResult.Text = "result";
      // 
      // buttonLoad
      // 
      this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoad.Location = new System.Drawing.Point(13, 439);
      this.buttonLoad.Name = "buttonLoad";
      this.buttonLoad.Size = new System.Drawing.Size(110, 23);
      this.buttonLoad.TabIndex = 13;
      this.buttonLoad.Text = "Load image";
      this.buttonLoad.UseVisualStyleBackColor = true;
      this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
      // 
      // checkDiff
      // 
      this.checkDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkDiff.AutoSize = true;
      this.checkDiff.Location = new System.Drawing.Point(501, 444);
      this.checkDiff.Name = "checkDiff";
      this.checkDiff.Size = new System.Drawing.Size(76, 17);
      this.checkDiff.TabIndex = 14;
      this.checkDiff.Text = "Show diff?";
      this.checkDiff.UseVisualStyleBackColor = true;
      this.checkDiff.CheckedChanged += new System.EventHandler(this.checkDiff_CheckedChanged);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(712, 474);
      this.Controls.Add(this.checkDiff);
      this.Controls.Add(this.buttonLoad);
      this.Controls.Add(this.labelResult);
      this.Controls.Add(this.labelElapsed);
      this.Controls.Add(this.buttonRecode);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(728, 200);
      this.Name = "Form1";
      this.Text = "011 compress B/W";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonRecode;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Label labelResult;
    private System.Windows.Forms.Button buttonLoad;
    private System.Windows.Forms.CheckBox checkDiff;
  }
}

