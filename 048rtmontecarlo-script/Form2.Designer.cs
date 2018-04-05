namespace _048rtmontecarlo
{
  partial class Form2
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
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
    private void InitializeComponent()
    {
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.SaveDepthMapButton = new System.Windows.Forms.Button();
      this.RenderDepthMapButton = new System.Windows.Forms.Button();
      this.DepthMapPictureBox = new System.Windows.Forms.PictureBox();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.SaveIntensityMapButton = new System.Windows.Forms.Button();
      this.IntensityMapPictureBox = new System.Windows.Forms.PictureBox();
      this.RenderIntensityMapButton = new System.Windows.Forms.Button();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.tabPage4 = new System.Windows.Forms.TabPage();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).BeginInit();
      this.tabPage2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.IntensityMapPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Controls.Add(this.tabPage3);
      this.tabControl1.Controls.Add(this.tabPage4);
      this.tabControl1.Location = new System.Drawing.Point(12, 12);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(805, 426);
      this.tabControl1.TabIndex = 0;
      this.tabControl1.Tag = "";
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.SaveDepthMapButton);
      this.tabPage1.Controls.Add(this.RenderDepthMapButton);
      this.tabPage1.Controls.Add(this.DepthMapPictureBox);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(797, 400);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Depth Map";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // SaveDepthMapButton
      // 
      this.SaveDepthMapButton.Location = new System.Drawing.Point(9, 353);
      this.SaveDepthMapButton.Name = "SaveDepthMapButton";
      this.SaveDepthMapButton.Size = new System.Drawing.Size(97, 41);
      this.SaveDepthMapButton.TabIndex = 2;
      this.SaveDepthMapButton.Text = "Save Image";
      this.SaveDepthMapButton.UseVisualStyleBackColor = true;
      this.SaveDepthMapButton.Click += new System.EventHandler(this.SaveDepthMapButton_Click);
      // 
      // RenderDepthMapButton
      // 
      this.RenderDepthMapButton.Location = new System.Drawing.Point(9, 6);
      this.RenderDepthMapButton.Name = "RenderDepthMapButton";
      this.RenderDepthMapButton.Size = new System.Drawing.Size(97, 41);
      this.RenderDepthMapButton.TabIndex = 1;
      this.RenderDepthMapButton.Text = "Render";
      this.RenderDepthMapButton.UseVisualStyleBackColor = true;
      this.RenderDepthMapButton.Click += new System.EventHandler(this.RenderDepthMapButton_Click);
      // 
      // DepthMapPictureBox
      // 
      this.DepthMapPictureBox.Location = new System.Drawing.Point(112, 6);
      this.DepthMapPictureBox.Name = "DepthMapPictureBox";
      this.DepthMapPictureBox.Size = new System.Drawing.Size(680, 360);
      this.DepthMapPictureBox.TabIndex = 0;
      this.DepthMapPictureBox.TabStop = false;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.SaveIntensityMapButton);
      this.tabPage2.Controls.Add(this.IntensityMapPictureBox);
      this.tabPage2.Controls.Add(this.RenderIntensityMapButton);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(797, 400);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Intensity Map";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // SaveIntensityMapButton
      // 
      this.SaveIntensityMapButton.Location = new System.Drawing.Point(9, 353);
      this.SaveIntensityMapButton.Name = "SaveIntensityMapButton";
      this.SaveIntensityMapButton.Size = new System.Drawing.Size(97, 41);
      this.SaveIntensityMapButton.TabIndex = 5;
      this.SaveIntensityMapButton.Text = "Save Image";
      this.SaveIntensityMapButton.UseVisualStyleBackColor = true;
      this.SaveIntensityMapButton.Click += new System.EventHandler(this.SaveIntensityMapButton_Click);
      // 
      // IntensityMapPictureBox
      // 
      this.IntensityMapPictureBox.Location = new System.Drawing.Point(112, 6);
      this.IntensityMapPictureBox.Name = "IntensityMapPictureBox";
      this.IntensityMapPictureBox.Size = new System.Drawing.Size(680, 360);
      this.IntensityMapPictureBox.TabIndex = 3;
      this.IntensityMapPictureBox.TabStop = false;
      // 
      // RenderIntensityMapButton
      // 
      this.RenderIntensityMapButton.Location = new System.Drawing.Point(9, 6);
      this.RenderIntensityMapButton.Name = "RenderIntensityMapButton";
      this.RenderIntensityMapButton.Size = new System.Drawing.Size(97, 41);
      this.RenderIntensityMapButton.TabIndex = 4;
      this.RenderIntensityMapButton.Text = "Render";
      this.RenderIntensityMapButton.UseVisualStyleBackColor = true;
      this.RenderIntensityMapButton.Click += new System.EventHandler(this.RenderIntensityMapButton_Click);
      // 
      // tabPage3
      // 
      this.tabPage3.Location = new System.Drawing.Point(4, 22);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Size = new System.Drawing.Size(797, 400);
      this.tabPage3.TabIndex = 2;
      this.tabPage3.Text = "tabPage3";
      this.tabPage3.UseVisualStyleBackColor = true;
      // 
      // tabPage4
      // 
      this.tabPage4.Location = new System.Drawing.Point(4, 22);
      this.tabPage4.Name = "tabPage4";
      this.tabPage4.Size = new System.Drawing.Size(797, 400);
      this.tabPage4.TabIndex = 3;
      this.tabPage4.Text = "tabPage4";
      this.tabPage4.UseVisualStyleBackColor = true;
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(837, 450);
      this.Controls.Add(this.tabControl1);
      this.Name = "Form2";
      this.Text = "Advanced view";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).EndInit();
      this.tabPage2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.IntensityMapPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TabPage tabPage4;
    internal System.Windows.Forms.PictureBox DepthMapPictureBox;
    private System.Windows.Forms.Button SaveDepthMapButton;
    private System.Windows.Forms.Button RenderDepthMapButton;
    private System.Windows.Forms.Button SaveIntensityMapButton;
    internal System.Windows.Forms.PictureBox IntensityMapPictureBox;
    private System.Windows.Forms.Button RenderIntensityMapButton;
  }
}