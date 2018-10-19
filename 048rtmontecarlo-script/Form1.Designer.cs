namespace Rendering
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
      this.components = new System.ComponentModel.Container();
      this.buttonSave = new System.Windows.Forms.Button();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.labelSample = new System.Windows.Forms.Label();
      this.buttonStop = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.comboScene = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.numericSupersampling = new System.Windows.Forms.NumericUpDown();
      this.checkShadows = new System.Windows.Forms.CheckBox();
      this.checkReflections = new System.Windows.Forms.CheckBox();
      this.checkRefractions = new System.Windows.Forms.CheckBox();
      this.checkJitter = new System.Windows.Forms.CheckBox();
      this.checkMultithreading = new System.Windows.Forms.CheckBox();
      this.buttonRes = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.textParam = new System.Windows.Forms.TextBox();
      this.AdvancedToolsButton = new System.Windows.Forms.Button();
      this.RayVisualiserButton = new System.Windows.Forms.Button();
      this.buttonRender = new wyDay.Controls.SplitButton();
      this.RenderButtonMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.addRenderClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.panel1 = new System.Windows.Forms.Panel();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.numericSupersampling)).BeginInit();
      this.RenderButtonMenuStrip.SuspendLayout();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(849, 581);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(91, 23);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point(206, 587);
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size(48, 13);
      this.labelElapsed.TabIndex = 21;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelSample
      // 
      this.labelSample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelSample.AutoSize = true;
      this.labelSample.Location = new System.Drawing.Point(677, 524);
      this.labelSample.Name = "labelSample";
      this.labelSample.Size = new System.Drawing.Size(45, 13);
      this.labelSample.TabIndex = 22;
      this.labelSample.Text = "Sample:";
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point(126, 581);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(65, 23);
      this.buttonStop.TabIndex = 32;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 522);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(41, 13);
      this.label1.TabIndex = 33;
      this.label1.Text = "Scene:";
      // 
      // comboScene
      // 
      this.comboScene.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboScene.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboScene.FormattingEnabled = true;
      this.comboScene.Location = new System.Drawing.Point(55, 518);
      this.comboScene.Name = "comboScene";
      this.comboScene.Size = new System.Drawing.Size(155, 21);
      this.comboScene.TabIndex = 34;
      this.comboScene.SelectedIndexChanged += new System.EventHandler(this.comboScene_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(123, 554);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(79, 13);
      this.label2.TabIndex = 35;
      this.label2.Text = "Supersampling:";
      // 
      // numericSupersampling
      // 
      this.numericSupersampling.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSupersampling.Location = new System.Drawing.Point(204, 551);
      this.numericSupersampling.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numericSupersampling.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericSupersampling.Name = "numericSupersampling";
      this.numericSupersampling.Size = new System.Drawing.Size(52, 20);
      this.numericSupersampling.TabIndex = 36;
      this.numericSupersampling.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // checkShadows
      // 
      this.checkShadows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkShadows.AutoSize = true;
      this.checkShadows.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkShadows.Checked = true;
      this.checkShadows.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkShadows.Location = new System.Drawing.Point(331, 553);
      this.checkShadows.Name = "checkShadows";
      this.checkShadows.Size = new System.Drawing.Size(68, 17);
      this.checkShadows.TabIndex = 37;
      this.checkShadows.Text = "shadows";
      this.checkShadows.UseVisualStyleBackColor = true;
      // 
      // checkReflections
      // 
      this.checkReflections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkReflections.AutoSize = true;
      this.checkReflections.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkReflections.Checked = true;
      this.checkReflections.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkReflections.Location = new System.Drawing.Point(405, 553);
      this.checkReflections.Name = "checkReflections";
      this.checkReflections.Size = new System.Drawing.Size(74, 17);
      this.checkReflections.TabIndex = 38;
      this.checkReflections.Text = "reflections";
      this.checkReflections.UseVisualStyleBackColor = true;
      // 
      // checkRefractions
      // 
      this.checkRefractions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkRefractions.AutoSize = true;
      this.checkRefractions.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkRefractions.Checked = true;
      this.checkRefractions.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkRefractions.Location = new System.Drawing.Point(485, 553);
      this.checkRefractions.Name = "checkRefractions";
      this.checkRefractions.Size = new System.Drawing.Size(75, 17);
      this.checkRefractions.TabIndex = 39;
      this.checkRefractions.Text = "refractions";
      this.checkRefractions.UseVisualStyleBackColor = true;
      // 
      // checkJitter
      // 
      this.checkJitter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkJitter.AutoSize = true;
      this.checkJitter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkJitter.Checked = true;
      this.checkJitter.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkJitter.Location = new System.Drawing.Point(266, 553);
      this.checkJitter.Name = "checkJitter";
      this.checkJitter.Size = new System.Drawing.Size(59, 17);
      this.checkJitter.TabIndex = 40;
      this.checkJitter.Text = "jittering";
      this.checkJitter.UseVisualStyleBackColor = true;
      // 
      // checkMultithreading
      // 
      this.checkMultithreading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkMultithreading.AutoSize = true;
      this.checkMultithreading.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkMultithreading.Location = new System.Drawing.Point(566, 553);
      this.checkMultithreading.Name = "checkMultithreading";
      this.checkMultithreading.Size = new System.Drawing.Size(94, 17);
      this.checkMultithreading.TabIndex = 41;
      this.checkMultithreading.Text = "multi-threading";
      this.checkMultithreading.UseVisualStyleBackColor = true;
      // 
      // buttonRes
      // 
      this.buttonRes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRes.Location = new System.Drawing.Point(13, 549);
      this.buttonRes.Name = "buttonRes";
      this.buttonRes.Size = new System.Drawing.Size(101, 23);
      this.buttonRes.TabIndex = 42;
      this.buttonRes.Text = "Resolution";
      this.buttonRes.UseVisualStyleBackColor = true;
      this.buttonRes.Click += new System.EventHandler(this.buttonRes_Click);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(218, 523);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 43;
      this.label3.Text = "Param:";
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(261, 519);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(410, 20);
      this.textParam.TabIndex = 44;
      this.textParam.TextChanged += new System.EventHandler(this.textParam_TextChanged);
      // 
      // AdvancedToolsButton
      // 
      this.AdvancedToolsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.AdvancedToolsButton.Location = new System.Drawing.Point(777, 581);
      this.AdvancedToolsButton.Name = "AdvancedToolsButton";
      this.AdvancedToolsButton.Size = new System.Drawing.Size(66, 23);
      this.AdvancedToolsButton.TabIndex = 45;
      this.AdvancedToolsButton.Text = "Advanced";
      this.AdvancedToolsButton.UseVisualStyleBackColor = true;
      this.AdvancedToolsButton.Click += new System.EventHandler(this.AdvancedToolsButton_Click);
      // 
      // RayVisualiserButton
      // 
      this.RayVisualiserButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.RayVisualiserButton.Location = new System.Drawing.Point(680, 581);
      this.RayVisualiserButton.Name = "RayVisualiserButton";
      this.RayVisualiserButton.Size = new System.Drawing.Size(91, 23);
      this.RayVisualiserButton.TabIndex = 46;
      this.RayVisualiserButton.Text = "Ray Visualiser";
      this.RayVisualiserButton.UseVisualStyleBackColor = true;
      this.RayVisualiserButton.Click += new System.EventHandler(this.RayVisualiserButton_Click);
      // 
      // buttonRender
      // 
      this.buttonRender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRender.AutoSize = true;
      this.buttonRender.ContextMenuStrip = this.RenderButtonMenuStrip;
      this.buttonRender.Location = new System.Drawing.Point(13, 581);
      this.buttonRender.Name = "buttonRender";
      this.buttonRender.Size = new System.Drawing.Size(101, 23);
      this.buttonRender.SplitMenuStrip = this.RenderButtonMenuStrip;
      this.buttonRender.TabIndex = 47;
      this.buttonRender.Text = "Render";
      this.buttonRender.UseVisualStyleBackColor = true;
      this.buttonRender.Click += new System.EventHandler(this.buttonRender_Click);
      // 
      // RenderButtonMenuStrip
      // 
      this.RenderButtonMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRenderClientToolStripMenuItem});
      this.RenderButtonMenuStrip.Name = "RenderButtonMenuStrip";
      this.RenderButtonMenuStrip.Size = new System.Drawing.Size(171, 26);
      // 
      // addRenderClientToolStripMenuItem
      // 
      this.addRenderClientToolStripMenuItem.Name = "addRenderClientToolStripMenuItem";
      this.addRenderClientToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
      this.addRenderClientToolStripMenuItem.Text = "Add Render Client";
      this.addRenderClientToolStripMenuItem.Click += new System.EventHandler(this.addRenderClientToolStripMenuItem_Click);
      // 
      // panel1
      // 
      this.panel1.AutoScroll = true;
      this.panel1.AutoSize = true;
      this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Controls.Add(this.flowLayoutPanel2);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(959, 460);
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(959, 460);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 50;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
      this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
      this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
      this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
      // 
      // flowLayoutPanel2
      // 
      this.flowLayoutPanel2.Location = new System.Drawing.Point(102, 45);
      this.flowLayoutPanel2.Name = "flowLayoutPanel2";
      this.flowLayoutPanel2.Size = new System.Drawing.Size(197, 122);
      this.flowLayoutPanel2.TabIndex = 48;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(959, 616);
      this.Controls.Add(this.buttonRender);
      this.Controls.Add(this.RayVisualiserButton);
      this.Controls.Add(this.AdvancedToolsButton);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.buttonRes);
      this.Controls.Add(this.checkMultithreading);
      this.Controls.Add(this.checkJitter);
      this.Controls.Add(this.checkRefractions);
      this.Controls.Add(this.checkReflections);
      this.Controls.Add(this.checkShadows);
      this.Controls.Add(this.numericSupersampling);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.comboScene);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.labelSample);
      this.Controls.Add(this.labelElapsed);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(690, 300);
      this.Name = "Form1";
      this.Text = "048 Monte Carlo RT script";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.numericSupersampling)).EndInit();
      this.RenderButtonMenuStrip.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Label labelSample;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox comboScene;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericSupersampling;
    private System.Windows.Forms.CheckBox checkShadows;
    private System.Windows.Forms.CheckBox checkReflections;
    private System.Windows.Forms.CheckBox checkRefractions;
    private System.Windows.Forms.CheckBox checkJitter;
    private System.Windows.Forms.CheckBox checkMultithreading;
    private System.Windows.Forms.Button buttonRes;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textParam;
    internal System.Windows.Forms.Button AdvancedToolsButton;
    internal System.Windows.Forms.Button RayVisualiserButton;
		private wyDay.Controls.SplitButton buttonRender;
		private System.Windows.Forms.ContextMenuStrip RenderButtonMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem addRenderClientToolStripMenuItem;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
  }
}

