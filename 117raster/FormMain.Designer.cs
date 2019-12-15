namespace _117raster
{
  partial class FormMain
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.pictureBoxMain = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.comboBoxModule = new System.Windows.Forms.ComboBox();
      this.buttonModule = new System.Windows.Forms.Button();
      this.labelStatus = new System.Windows.Forms.Label();
      this.buttonLoadImage = new System.Windows.Forms.Button();
      this.buttonSaveImage = new System.Windows.Forms.Button();
      this.buttonRecompute = new System.Windows.Forms.Button();
      this.checkBoxResult = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textBoxParam = new System.Windows.Forms.TextBox();
      this.buttonSetInput = new System.Windows.Forms.Button();
      this.buttonZoomReset = new System.Windows.Forms.Button();
      this.buttonShowGUI = new System.Windows.Forms.Button();
      this.buttonBreak = new System.Windows.Forms.Button();
      this.buttonClear = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBoxMain
      // 
      this.pictureBoxMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBoxMain.Location = new System.Drawing.Point(5, 5);
      this.pictureBoxMain.Name = "pictureBoxMain";
      this.pictureBoxMain.Size = new System.Drawing.Size(874, 450);
      this.pictureBoxMain.TabIndex = 0;
      this.pictureBoxMain.TabStop = false;
      this.pictureBoxMain.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxMain_Paint);
      this.pictureBoxMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMain_MouseDown);
      this.pictureBoxMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMain_MouseMove);
      this.pictureBoxMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMain_MouseUp);
      this.pictureBoxMain.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pictureBoxMain_PreviewKeyDown);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 472);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(45, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Module:";
      // 
      // comboBoxModule
      // 
      this.comboBoxModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboBoxModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxModule.Location = new System.Drawing.Point(62, 469);
      this.comboBoxModule.Name = "comboBoxModule";
      this.comboBoxModule.Size = new System.Drawing.Size(346, 21);
      this.comboBoxModule.Sorted = true;
      this.comboBoxModule.TabIndex = 1;
      this.comboBoxModule.SelectedIndexChanged += new System.EventHandler(this.comboBoxModule_SelectedIndexChanged);
      // 
      // buttonModule
      // 
      this.buttonModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonModule.Location = new System.Drawing.Point(105, 506);
      this.buttonModule.Name = "buttonModule";
      this.buttonModule.Size = new System.Drawing.Size(105, 23);
      this.buttonModule.TabIndex = 5;
      this.buttonModule.Text = "Activate module";
      this.buttonModule.UseVisualStyleBackColor = true;
      this.buttonModule.Click += new System.EventHandler(this.buttonModule_Click);
      // 
      // labelStatus
      // 
      this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelStatus.AutoEllipsis = true;
      this.labelStatus.Location = new System.Drawing.Point(493, 472);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(377, 20);
      this.labelStatus.TabIndex = 3;
      this.labelStatus.Text = "---  status ---";
      this.labelStatus.MouseHover += new System.EventHandler(this.labelStatus_MouseHover);
      // 
      // buttonLoadImage
      // 
      this.buttonLoadImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoadImage.Location = new System.Drawing.Point(16, 506);
      this.buttonLoadImage.Name = "buttonLoadImage";
      this.buttonLoadImage.Size = new System.Drawing.Size(79, 23);
      this.buttonLoadImage.TabIndex = 4;
      this.buttonLoadImage.Text = "Load image";
      this.buttonLoadImage.UseVisualStyleBackColor = true;
      this.buttonLoadImage.Click += new System.EventHandler(this.buttonLoadImage_Click);
      // 
      // buttonSaveImage
      // 
      this.buttonSaveImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSaveImage.Enabled = false;
      this.buttonSaveImage.Location = new System.Drawing.Point(784, 544);
      this.buttonSaveImage.Name = "buttonSaveImage";
      this.buttonSaveImage.Size = new System.Drawing.Size(86, 23);
      this.buttonSaveImage.TabIndex = 14;
      this.buttonSaveImage.Text = "Save image";
      this.buttonSaveImage.UseVisualStyleBackColor = true;
      this.buttonSaveImage.Click += new System.EventHandler(this.buttonSaveImage_Click);
      // 
      // buttonRecompute
      // 
      this.buttonRecompute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRecompute.Enabled = false;
      this.buttonRecompute.Location = new System.Drawing.Point(220, 506);
      this.buttonRecompute.Name = "buttonRecompute";
      this.buttonRecompute.Size = new System.Drawing.Size(82, 23);
      this.buttonRecompute.TabIndex = 6;
      this.buttonRecompute.Text = "Recompute";
      this.buttonRecompute.UseVisualStyleBackColor = true;
      this.buttonRecompute.Click += new System.EventHandler(this.buttonRecompute_Click);
      // 
      // checkBoxResult
      // 
      this.checkBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkBoxResult.AutoSize = true;
      this.checkBoxResult.Location = new System.Drawing.Point(426, 472);
      this.checkBoxResult.Name = "checkBoxResult";
      this.checkBoxResult.Size = new System.Drawing.Size(51, 17);
      this.checkBoxResult.TabIndex = 2;
      this.checkBoxResult.Text = "result";
      this.checkBoxResult.UseVisualStyleBackColor = true;
      this.checkBoxResult.CheckedChanged += new System.EventHandler(this.checkBoxResult_CheckedChanged);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(423, 511);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(40, 13);
      this.label2.TabIndex = 8;
      this.label2.Text = "Param:";
      // 
      // textBoxParam
      // 
      this.textBoxParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxParam.Location = new System.Drawing.Point(469, 508);
      this.textBoxParam.Name = "textBoxParam";
      this.textBoxParam.Size = new System.Drawing.Size(401, 20);
      this.textBoxParam.TabIndex = 9;
      this.textBoxParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxParam_KeyPress);
      this.textBoxParam.MouseHover += new System.EventHandler(this.textBoxParam_MouseHover);
      // 
      // buttonSetInput
      // 
      this.buttonSetInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonSetInput.Enabled = false;
      this.buttonSetInput.Location = new System.Drawing.Point(312, 506);
      this.buttonSetInput.Name = "buttonSetInput";
      this.buttonSetInput.Size = new System.Drawing.Size(95, 23);
      this.buttonSetInput.TabIndex = 7;
      this.buttonSetInput.Text = "Result -> input";
      this.buttonSetInput.UseVisualStyleBackColor = true;
      this.buttonSetInput.Click += new System.EventHandler(this.buttonSetInput_Click);
      // 
      // buttonZoomReset
      // 
      this.buttonZoomReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonZoomReset.Location = new System.Drawing.Point(16, 544);
      this.buttonZoomReset.Name = "buttonZoomReset";
      this.buttonZoomReset.Size = new System.Drawing.Size(79, 23);
      this.buttonZoomReset.TabIndex = 10;
      this.buttonZoomReset.Text = "Zoom 100%";
      this.buttonZoomReset.UseVisualStyleBackColor = true;
      this.buttonZoomReset.Click += new System.EventHandler(this.buttonZoomReset_Click);
      // 
      // buttonShowGUI
      // 
      this.buttonShowGUI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonShowGUI.Enabled = false;
      this.buttonShowGUI.Location = new System.Drawing.Point(220, 544);
      this.buttonShowGUI.Name = "buttonShowGUI";
      this.buttonShowGUI.Size = new System.Drawing.Size(82, 23);
      this.buttonShowGUI.TabIndex = 12;
      this.buttonShowGUI.Text = "Module GUI";
      this.buttonShowGUI.UseVisualStyleBackColor = true;
      this.buttonShowGUI.Click += new System.EventHandler(this.buttonShowGUI_Click);
      // 
      // buttonBreak
      // 
      this.buttonBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonBreak.Enabled = false;
      this.buttonBreak.Location = new System.Drawing.Point(312, 544);
      this.buttonBreak.Name = "buttonBreak";
      this.buttonBreak.Size = new System.Drawing.Size(95, 23);
      this.buttonBreak.TabIndex = 13;
      this.buttonBreak.Text = "Break";
      this.buttonBreak.UseVisualStyleBackColor = true;
      this.buttonBreak.Click += new System.EventHandler(this.buttonBreak_Click);
      // 
      // buttonClear
      // 
      this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonClear.Enabled = false;
      this.buttonClear.Location = new System.Drawing.Point(105, 544);
      this.buttonClear.Name = "buttonClear";
      this.buttonClear.Size = new System.Drawing.Size(105, 23);
      this.buttonClear.TabIndex = 11;
      this.buttonClear.Text = "Clear input";
      this.buttonClear.UseVisualStyleBackColor = true;
      this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
      // 
      // FormMain
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(884, 581);
      this.Controls.Add(this.buttonClear);
      this.Controls.Add(this.buttonBreak);
      this.Controls.Add(this.buttonShowGUI);
      this.Controls.Add(this.buttonZoomReset);
      this.Controls.Add(this.buttonSetInput);
      this.Controls.Add(this.textBoxParam);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.checkBoxResult);
      this.Controls.Add(this.buttonRecompute);
      this.Controls.Add(this.buttonSaveImage);
      this.Controls.Add(this.buttonLoadImage);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.buttonModule);
      this.Controls.Add(this.comboBoxModule);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.pictureBoxMain);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(650, 300);
      this.Name = "FormMain";
      this.Text = "117 raster";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
      this.Load += new System.EventHandler(this.FormMain_Load);
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormMain_DragDrop);
      this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormMain_DragEnter);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBoxMain;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox comboBoxModule;
    private System.Windows.Forms.Button buttonModule;
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.Button buttonLoadImage;
    private System.Windows.Forms.Button buttonSaveImage;
    private System.Windows.Forms.Button buttonRecompute;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBoxParam;
    private System.Windows.Forms.Button buttonSetInput;
    private System.Windows.Forms.Button buttonZoomReset;
    private System.Windows.Forms.Button buttonShowGUI;
    private System.Windows.Forms.CheckBox checkBoxResult;
    private System.Windows.Forms.Button buttonBreak;
    private System.Windows.Forms.Button buttonClear;
  }
}

