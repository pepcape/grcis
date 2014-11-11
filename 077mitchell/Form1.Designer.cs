namespace _077mitchell
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
      this.elementHostCanvas = new System.Windows.Forms.Integration.ElementHost();
      this.comboSampling = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.numericSeed = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.textParams = new System.Windows.Forms.TextBox();
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.numericSamples = new System.Windows.Forms.NumericUpDown();
      this.labelStatus = new System.Windows.Forms.Label();
      this.buttonStop = new System.Windows.Forms.Button();
      this.buttonSaveSamples = new System.Windows.Forms.Button();
      this.buttonExportImage = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.numericResolution = new System.Windows.Forms.NumericUpDown();
      this.buttonExportDrawing = new System.Windows.Forms.Button();
      this.buttonResize = new System.Windows.Forms.Button();
      this.label8 = new System.Windows.Forms.Label();
      this.comboDensity = new System.Windows.Forms.ComboBox();
      this.buttonDensityFile = new System.Windows.Forms.Button();
      this.checkNegative = new System.Windows.Forms.CheckBox();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.labelDrawing = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSamples)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericResolution)).BeginInit();
      this.SuspendLayout();
      // 
      // elementHostCanvas
      // 
      this.elementHostCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.elementHostCanvas.Location = new System.Drawing.Point(13, 13);
      this.elementHostCanvas.Name = "elementHostCanvas";
      this.elementHostCanvas.Size = new System.Drawing.Size(635, 297);
      this.elementHostCanvas.TabIndex = 0;
      this.elementHostCanvas.Text = "elementHostCanvas";
      this.elementHostCanvas.Child = null;
      // 
      // comboSampling
      // 
      this.comboSampling.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboSampling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboSampling.FormattingEnabled = true;
      this.comboSampling.Location = new System.Drawing.Point(64, 325);
      this.comboSampling.Name = "comboSampling";
      this.comboSampling.Size = new System.Drawing.Size(163, 21);
      this.comboSampling.TabIndex = 36;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(11, 330);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(46, 13);
      this.label1.TabIndex = 35;
      this.label1.Text = "Method:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 366);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(45, 13);
      this.label2.TabIndex = 37;
      this.label2.Text = "Params:";
      // 
      // numericSeed
      // 
      this.numericSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSeed.Location = new System.Drawing.Point(417, 328);
      this.numericSeed.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
      this.numericSeed.Name = "numericSeed";
      this.numericSeed.Size = new System.Drawing.Size(93, 20);
      this.numericSeed.TabIndex = 38;
      this.numericSeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(378, 330);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(35, 13);
      this.label3.TabIndex = 39;
      this.label3.Text = "Seed:";
      // 
      // textParams
      // 
      this.textParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParams.Location = new System.Drawing.Point(64, 364);
      this.textParams.Name = "textParams";
      this.textParams.Size = new System.Drawing.Size(446, 20);
      this.textParams.TabIndex = 40;
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonGenerate.Location = new System.Drawing.Point(669, 13);
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size(93, 23);
      this.buttonGenerate.TabIndex = 41;
      this.buttonGenerate.Text = "Generate";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(233, 330);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(50, 13);
      this.label4.TabIndex = 43;
      this.label4.Text = "Samples:";
      // 
      // numericSamples
      // 
      this.numericSamples.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericSamples.Location = new System.Drawing.Point(287, 327);
      this.numericSamples.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
      this.numericSamples.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericSamples.Name = "numericSamples";
      this.numericSamples.Size = new System.Drawing.Size(76, 20);
      this.numericSamples.TabIndex = 42;
      this.numericSamples.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.numericSamples.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(11, 432);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(38, 13);
      this.labelStatus.TabIndex = 44;
      this.labelStatus.Text = "Ready";
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonStop.Location = new System.Drawing.Point(669, 42);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(93, 23);
      this.buttonStop.TabIndex = 45;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // buttonSaveSamples
      // 
      this.buttonSaveSamples.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSaveSamples.Location = new System.Drawing.Point(669, 430);
      this.buttonSaveSamples.Name = "buttonSaveSamples";
      this.buttonSaveSamples.Size = new System.Drawing.Size(93, 23);
      this.buttonSaveSamples.TabIndex = 48;
      this.buttonSaveSamples.Text = "Save samples";
      this.buttonSaveSamples.UseVisualStyleBackColor = true;
      this.buttonSaveSamples.Click += new System.EventHandler(this.buttonSaveSamples_Click);
      // 
      // buttonExportImage
      // 
      this.buttonExportImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonExportImage.Location = new System.Drawing.Point(669, 398);
      this.buttonExportImage.Name = "buttonExportImage";
      this.buttonExportImage.Size = new System.Drawing.Size(93, 23);
      this.buttonExportImage.TabIndex = 51;
      this.buttonExportImage.Text = "Export image";
      this.buttonExportImage.UseVisualStyleBackColor = true;
      this.buttonExportImage.Click += new System.EventHandler(this.buttonExportImage_Click);
      // 
      // label6
      // 
      this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(524, 332);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(60, 13);
      this.label6.TabIndex = 55;
      this.label6.Text = "Resolution:";
      // 
      // numericResolution
      // 
      this.numericResolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericResolution.Location = new System.Drawing.Point(590, 328);
      this.numericResolution.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
      this.numericResolution.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericResolution.Name = "numericResolution";
      this.numericResolution.Size = new System.Drawing.Size(58, 20);
      this.numericResolution.TabIndex = 54;
      this.numericResolution.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.numericResolution.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
      // 
      // buttonExportDrawing
      // 
      this.buttonExportDrawing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonExportDrawing.Location = new System.Drawing.Point(669, 366);
      this.buttonExportDrawing.Name = "buttonExportDrawing";
      this.buttonExportDrawing.Size = new System.Drawing.Size(93, 23);
      this.buttonExportDrawing.TabIndex = 63;
      this.buttonExportDrawing.Text = "Export drawing";
      this.buttonExportDrawing.UseVisualStyleBackColor = true;
      this.buttonExportDrawing.Click += new System.EventHandler(this.buttonExportDrawing_Click);
      // 
      // buttonResize
      // 
      this.buttonResize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonResize.Location = new System.Drawing.Point(669, 265);
      this.buttonResize.Name = "buttonResize";
      this.buttonResize.Size = new System.Drawing.Size(93, 23);
      this.buttonResize.TabIndex = 64;
      this.buttonResize.Text = "Resize";
      this.buttonResize.UseVisualStyleBackColor = true;
      this.buttonResize.Click += new System.EventHandler(this.buttonResize_Click);
      // 
      // label8
      // 
      this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(11, 401);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(45, 13);
      this.label8.TabIndex = 68;
      this.label8.Text = "Density:";
      // 
      // comboDensity
      // 
      this.comboDensity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboDensity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboDensity.FormattingEnabled = true;
      this.comboDensity.Location = new System.Drawing.Point(64, 398);
      this.comboDensity.Name = "comboDensity";
      this.comboDensity.Size = new System.Drawing.Size(121, 21);
      this.comboDensity.TabIndex = 69;
      // 
      // buttonDensityFile
      // 
      this.buttonDensityFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonDensityFile.Location = new System.Drawing.Point(199, 398);
      this.buttonDensityFile.Name = "buttonDensityFile";
      this.buttonDensityFile.Size = new System.Drawing.Size(186, 23);
      this.buttonDensityFile.TabIndex = 70;
      this.buttonDensityFile.Text = "Density file";
      this.buttonDensityFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonDensityFile.UseVisualStyleBackColor = true;
      this.buttonDensityFile.Click += new System.EventHandler(this.buttonDensityFile_Click);
      // 
      // checkNegative
      // 
      this.checkNegative.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkNegative.AutoSize = true;
      this.checkNegative.Checked = true;
      this.checkNegative.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkNegative.Location = new System.Drawing.Point(394, 403);
      this.checkNegative.Name = "checkNegative";
      this.checkNegative.Size = new System.Drawing.Size(67, 17);
      this.checkNegative.TabIndex = 71;
      this.checkNegative.Text = "negative";
      this.checkNegative.UseVisualStyleBackColor = true;
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRedraw.Location = new System.Drawing.Point(669, 236);
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size(93, 23);
      this.buttonRedraw.TabIndex = 72;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler(this.buttonRedraw_Click);
      // 
      // labelDrawing
      // 
      this.labelDrawing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.labelDrawing.AutoSize = true;
      this.labelDrawing.Location = new System.Drawing.Point(673, 298);
      this.labelDrawing.Name = "labelDrawing";
      this.labelDrawing.Size = new System.Drawing.Size(44, 13);
      this.labelDrawing.TabIndex = 73;
      this.labelDrawing.Text = "drawing";
      this.labelDrawing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(774, 462);
      this.Controls.Add(this.labelDrawing);
      this.Controls.Add(this.buttonRedraw);
      this.Controls.Add(this.checkNegative);
      this.Controls.Add(this.buttonDensityFile);
      this.Controls.Add(this.comboDensity);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.buttonResize);
      this.Controls.Add(this.buttonExportDrawing);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.numericResolution);
      this.Controls.Add(this.buttonExportImage);
      this.Controls.Add(this.buttonSaveSamples);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.numericSamples);
      this.Controls.Add(this.buttonGenerate);
      this.Controls.Add(this.textParams);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.numericSeed);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.comboSampling);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.elementHostCanvas);
      this.MinimumSize = new System.Drawing.Size(790, 400);
      this.Name = "Form1";
      this.Text = "Mitchell";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.Resize += new System.EventHandler(this.Form1_Resize);
      ((System.ComponentModel.ISupportInitialize)(this.numericSeed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericSamples)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericResolution)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Integration.ElementHost elementHostCanvas;
    private System.Windows.Forms.ComboBox comboSampling;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericSeed;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textParams;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown numericSamples;
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.Button buttonSaveSamples;
    private System.Windows.Forms.Button buttonExportImage;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown numericResolution;
    private System.Windows.Forms.Button buttonExportDrawing;
    private System.Windows.Forms.Button buttonResize;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ComboBox comboDensity;
    private System.Windows.Forms.Button buttonDensityFile;
    private System.Windows.Forms.CheckBox checkNegative;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.Label labelDrawing;
  }
}

