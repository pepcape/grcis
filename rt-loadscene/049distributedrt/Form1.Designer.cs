namespace _049distributedrt
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
      this.buttonRender = new System.Windows.Forms.Button();
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
      this.textParam = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSupersampling)).BeginInit();
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
      this.panel1.Size = new System.Drawing.Size(680, 360);
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(680, 360);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
      this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(602, 447);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(91, 23);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // buttonRender
      // 
      this.buttonRender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRender.Location = new System.Drawing.Point(13, 447);
      this.buttonRender.Name = "buttonRender";
      this.buttonRender.Size = new System.Drawing.Size(103, 23);
      this.buttonRender.TabIndex = 5;
      this.buttonRender.Text = "Render";
      this.buttonRender.UseVisualStyleBackColor = true;
      this.buttonRender.Click += new System.EventHandler(this.buttonRender_Click);
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point(206, 452);
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size(48, 13);
      this.labelElapsed.TabIndex = 21;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelSample
      // 
      this.labelSample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelSample.AutoSize = true;
      this.labelSample.Location = new System.Drawing.Point(427, 390);
      this.labelSample.Name = "labelSample";
      this.labelSample.Size = new System.Drawing.Size(45, 13);
      this.labelSample.TabIndex = 22;
      this.labelSample.Text = "Sample:";
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point(126, 447);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(66, 23);
      this.buttonStop.TabIndex = 32;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 388);
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
      this.comboScene.Location = new System.Drawing.Point(58, 384);
      this.comboScene.Name = "comboScene";
      this.comboScene.Size = new System.Drawing.Size(155, 21);
      this.comboScene.TabIndex = 34;
      this.comboScene.SelectedIndexChanged += new System.EventHandler(this.comboScene_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(123, 420);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(79, 13);
      this.label2.TabIndex = 35;
      this.label2.Text = "Supersampling:";
      // 
      // numericSupersampling
      // 
      this.numericSupersampling.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSupersampling.Location = new System.Drawing.Point(204, 417);
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
      this.numericSupersampling.Size = new System.Drawing.Size(50, 20);
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
      this.checkShadows.Location = new System.Drawing.Point(326, 419);
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
      this.checkReflections.Location = new System.Drawing.Point(399, 419);
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
      this.checkRefractions.Location = new System.Drawing.Point(478, 419);
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
      this.checkJitter.Location = new System.Drawing.Point(262, 419);
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
      this.checkMultithreading.Location = new System.Drawing.Point(558, 419);
      this.checkMultithreading.Name = "checkMultithreading";
      this.checkMultithreading.Size = new System.Drawing.Size(94, 17);
      this.checkMultithreading.TabIndex = 41;
      this.checkMultithreading.Text = "multi-threading";
      this.checkMultithreading.UseVisualStyleBackColor = true;
      // 
      // buttonRes
      // 
      this.buttonRes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRes.Location = new System.Drawing.Point(14, 415);
      this.buttonRes.Name = "buttonRes";
      this.buttonRes.Size = new System.Drawing.Size(101, 23);
      this.buttonRes.TabIndex = 42;
      this.buttonRes.Text = "Resolution";
      this.buttonRes.UseVisualStyleBackColor = true;
      this.buttonRes.Click += new System.EventHandler(this.buttonRes_Click);
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textParam.Location = new System.Drawing.Point(265, 385);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(156, 20);
      this.textParam.TabIndex = 46;
      this.textParam.TextChanged += new System.EventHandler(this.textParam_TextChanged);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(222, 389);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 45;
      this.label3.Text = "Param:";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(712, 482);
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
      this.Controls.Add(this.buttonRender);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(690, 300);
      this.Name = "Form1";
      this.Text = "049 Distributed RT";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSupersampling)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonRender;
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
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Label label3;
  }
}

