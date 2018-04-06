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
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.tabPage4 = new System.Windows.Forms.TabPage();
      this.DepthMapPanel = new System.Windows.Forms.Panel();
      this.SaveDepthMapButton = new System.Windows.Forms.Button();
      this.RenderDepthMapButton = new System.Windows.Forms.Button();
      this.DepthMapPictureBox = new System.Windows.Forms.PictureBox();
      this.IntensityMapPanel = new System.Windows.Forms.Panel();
      this.SaveIntensityMapButton = new System.Windows.Forms.Button();
      this.IntensityMapPictureBox = new System.Windows.Forms.PictureBox();
      this.RenderIntensityMapButton = new System.Windows.Forms.Button();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.DepthMapPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.IntensityMapPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Controls.Add(this.tabPage3);
      this.tabControl1.Controls.Add(this.tabPage4);
      this.tabControl1.Location = new System.Drawing.Point(12, 12);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(819, 521);
      this.tabControl1.TabIndex = 0;
      this.tabControl1.Tag = "";
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.DepthMapPanel);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(811, 495);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Depth Map";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.SaveIntensityMapButton);
      this.tabPage2.Controls.Add(this.IntensityMapPictureBox);
      this.tabPage2.Controls.Add(this.RenderIntensityMapButton);
      this.tabPage2.Controls.Add(this.IntensityMapPanel);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(811, 495);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Intensity Map";
      this.tabPage2.UseVisualStyleBackColor = true;
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
      // DepthMapPanel
      // 
      this.DepthMapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DepthMapPanel.AutoScroll = true;
      this.DepthMapPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.DepthMapPanel.Controls.Add(this.SaveDepthMapButton);
      this.DepthMapPanel.Controls.Add(this.RenderDepthMapButton);
      this.DepthMapPanel.Controls.Add(this.DepthMapPictureBox);
      this.DepthMapPanel.Location = new System.Drawing.Point(0, 0);
      this.DepthMapPanel.Name = "DepthMapPanel";
      this.DepthMapPanel.Size = new System.Drawing.Size(811, 495);
      this.DepthMapPanel.TabIndex = 3;
      // 
      // SaveDepthMapButton
      // 
      this.SaveDepthMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.SaveDepthMapButton.Enabled = false;
      this.SaveDepthMapButton.Location = new System.Drawing.Point(7, 448);
      this.SaveDepthMapButton.Name = "SaveDepthMapButton";
      this.SaveDepthMapButton.Size = new System.Drawing.Size(97, 41);
      this.SaveDepthMapButton.TabIndex = 5;
      this.SaveDepthMapButton.Text = "Save Image";
      this.SaveDepthMapButton.UseVisualStyleBackColor = true;
      this.SaveDepthMapButton.Click += new System.EventHandler(this.SaveDepthMapButton_Click);
      // 
      // RenderDepthMapButton
      // 
      this.RenderDepthMapButton.Location = new System.Drawing.Point(7, 6);
      this.RenderDepthMapButton.Name = "RenderDepthMapButton";
      this.RenderDepthMapButton.Size = new System.Drawing.Size(97, 41);
      this.RenderDepthMapButton.TabIndex = 4;
      this.RenderDepthMapButton.Text = "Render";
      this.RenderDepthMapButton.UseVisualStyleBackColor = true;
      this.RenderDepthMapButton.Click += new System.EventHandler(this.RenderDepthMapButton_Click);
      // 
      // DepthMapPictureBox
      // 
      this.DepthMapPictureBox.Location = new System.Drawing.Point(110, 6);
      this.DepthMapPictureBox.Name = "DepthMapPictureBox";
      this.DepthMapPictureBox.Size = new System.Drawing.Size(680, 480);
      this.DepthMapPictureBox.TabIndex = 3;
      this.DepthMapPictureBox.TabStop = false;
      // 
      // IntensityMapPanel
      // 
      this.IntensityMapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.IntensityMapPanel.AutoScroll = true;
      this.IntensityMapPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.IntensityMapPanel.Location = new System.Drawing.Point(0, 0);
      this.IntensityMapPanel.Name = "IntensityMapPanel";
      this.IntensityMapPanel.Size = new System.Drawing.Size(811, 495);
      this.IntensityMapPanel.TabIndex = 6;
      // 
      // SaveIntensityMapButton
      // 
      this.SaveIntensityMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.SaveIntensityMapButton.Enabled = false;
      this.SaveIntensityMapButton.Location = new System.Drawing.Point(7, 448);
      this.SaveIntensityMapButton.Name = "SaveIntensityMapButton";
      this.SaveIntensityMapButton.Size = new System.Drawing.Size(97, 41);
      this.SaveIntensityMapButton.TabIndex = 9;
      this.SaveIntensityMapButton.Text = "Save Image";
      this.SaveIntensityMapButton.UseVisualStyleBackColor = true;
      this.SaveIntensityMapButton.Click += new System.EventHandler(this.SaveIntensityMapButton_Click);
      // 
      // IntensityMapPictureBox
      // 
      this.IntensityMapPictureBox.Location = new System.Drawing.Point(110, 6);
      this.IntensityMapPictureBox.Name = "IntensityMapPictureBox";
      this.IntensityMapPictureBox.Size = new System.Drawing.Size(680, 480);
      this.IntensityMapPictureBox.TabIndex = 7;
      this.IntensityMapPictureBox.TabStop = false;
      // 
      // RenderIntensityMapButton
      // 
      this.RenderIntensityMapButton.Location = new System.Drawing.Point(7, 6);
      this.RenderIntensityMapButton.Name = "RenderIntensityMapButton";
      this.RenderIntensityMapButton.Size = new System.Drawing.Size(97, 41);
      this.RenderIntensityMapButton.TabIndex = 8;
      this.RenderIntensityMapButton.Text = "Render";
      this.RenderIntensityMapButton.UseVisualStyleBackColor = true;
      this.RenderIntensityMapButton.Click += new System.EventHandler(this.RenderIntensityMapButton_Click);
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(851, 545);
      this.Controls.Add(this.tabControl1);
      this.Name = "Form2";
      this.Text = "Advanced tools";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.DepthMapPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.IntensityMapPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TabPage tabPage4;
    private System.Windows.Forms.Panel DepthMapPanel;
    private System.Windows.Forms.Button SaveDepthMapButton;
    private System.Windows.Forms.Button RenderDepthMapButton;
    internal System.Windows.Forms.PictureBox DepthMapPictureBox;
    private System.Windows.Forms.Button SaveIntensityMapButton;
    internal System.Windows.Forms.PictureBox IntensityMapPictureBox;
    private System.Windows.Forms.Button RenderIntensityMapButton;
    private System.Windows.Forms.Panel IntensityMapPanel;
  }
}