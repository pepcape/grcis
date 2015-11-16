namespace _085segmentation
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
      this.buttonOpen = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.tableImages = new System.Windows.Forms.TableLayoutPanel();
      this.pictureTarget = new System.Windows.Forms.PictureBox();
      this.buttonRecompute = new System.Windows.Forms.Button();
      this.buttonLoadMask = new System.Windows.Forms.Button();
      this.buttonSaveMask = new System.Windows.Forms.Button();
      this.buttonReset = new System.Windows.Forms.Button();
      this.numericPen = new System.Windows.Forms.NumericUpDown();
      this.labelPen = new System.Windows.Forms.Label();
      this.checkBoxWhite = new System.Windows.Forms.CheckBox();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.pictureSource = new _085segmentation.GUIPictureBox();
      this.tableImages.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericPen)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(12, 450);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(95, 23);
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load image";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(800, 450);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(94, 23);
      this.buttonSave.TabIndex = 10;
      this.buttonSave.Text = "Save result";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // tableImages
      // 
      this.tableImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableImages.ColumnCount = 2;
      this.tableImages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableImages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableImages.Controls.Add(this.pictureSource, 0, 0);
      this.tableImages.Controls.Add(this.pictureTarget, 1, 0);
      this.tableImages.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
      this.tableImages.Location = new System.Drawing.Point(12, 12);
      this.tableImages.Name = "tableImages";
      this.tableImages.RowCount = 1;
      this.tableImages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableImages.Size = new System.Drawing.Size(880, 426);
      this.tableImages.TabIndex = 0;
      // 
      // pictureTarget
      // 
      this.pictureTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureTarget.Location = new System.Drawing.Point(443, 3);
      this.pictureTarget.Name = "pictureTarget";
      this.pictureTarget.Size = new System.Drawing.Size(434, 420);
      this.pictureTarget.TabIndex = 6;
      this.pictureTarget.TabStop = false;
      // 
      // buttonRecompute
      // 
      this.buttonRecompute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRecompute.Location = new System.Drawing.Point(666, 450);
      this.buttonRecompute.Name = "buttonRecompute";
      this.buttonRecompute.Size = new System.Drawing.Size(122, 23);
      this.buttonRecompute.TabIndex = 9;
      this.buttonRecompute.Text = "Recompute";
      this.buttonRecompute.UseVisualStyleBackColor = true;
      this.buttonRecompute.Click += new System.EventHandler(this.buttonRecompute_Click);
      // 
      // buttonLoadMask
      // 
      this.buttonLoadMask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoadMask.Location = new System.Drawing.Point(198, 450);
      this.buttonLoadMask.Name = "buttonLoadMask";
      this.buttonLoadMask.Size = new System.Drawing.Size(75, 23);
      this.buttonLoadMask.TabIndex = 3;
      this.buttonLoadMask.Text = "Load mask";
      this.buttonLoadMask.UseVisualStyleBackColor = true;
      this.buttonLoadMask.Click += new System.EventHandler(this.buttonLoadMask_Click);
      // 
      // buttonSaveMask
      // 
      this.buttonSaveMask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonSaveMask.Location = new System.Drawing.Point(286, 450);
      this.buttonSaveMask.Name = "buttonSaveMask";
      this.buttonSaveMask.Size = new System.Drawing.Size(75, 23);
      this.buttonSaveMask.TabIndex = 4;
      this.buttonSaveMask.Text = "Save mask";
      this.buttonSaveMask.UseVisualStyleBackColor = true;
      this.buttonSaveMask.Click += new System.EventHandler(this.buttonSaveMask_Click);
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonReset.Location = new System.Drawing.Point(119, 450);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new System.Drawing.Size(65, 23);
      this.buttonReset.TabIndex = 2;
      this.buttonReset.Text = "Reset";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
      // 
      // numericPen
      // 
      this.numericPen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericPen.DecimalPlaces = 2;
      this.numericPen.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
      this.numericPen.Location = new System.Drawing.Point(401, 452);
      this.numericPen.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numericPen.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericPen.Name = "numericPen";
      this.numericPen.Size = new System.Drawing.Size(53, 20);
      this.numericPen.TabIndex = 6;
      this.numericPen.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
      this.numericPen.ValueChanged += new System.EventHandler(this.numericPen_ValueChanged);
      // 
      // labelPen
      // 
      this.labelPen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelPen.AutoSize = true;
      this.labelPen.Location = new System.Drawing.Point(367, 455);
      this.labelPen.Name = "labelPen";
      this.labelPen.Size = new System.Drawing.Size(29, 13);
      this.labelPen.TabIndex = 5;
      this.labelPen.Text = "Pen:";
      // 
      // checkBoxWhite
      // 
      this.checkBoxWhite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkBoxWhite.AutoSize = true;
      this.checkBoxWhite.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkBoxWhite.Checked = true;
      this.checkBoxWhite.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBoxWhite.Location = new System.Drawing.Point(461, 455);
      this.checkBoxWhite.Name = "checkBoxWhite";
      this.checkBoxWhite.Size = new System.Drawing.Size(54, 17);
      this.checkBoxWhite.TabIndex = 7;
      this.checkBoxWhite.Text = "White";
      this.checkBoxWhite.UseVisualStyleBackColor = true;
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point(525, 456);
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size(48, 13);
      this.labelElapsed.TabIndex = 8;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // pictureSource
      // 
      this.pictureSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureSource.Location = new System.Drawing.Point(3, 3);
      this.pictureSource.Name = "pictureSource";
      this.pictureSource.Size = new System.Drawing.Size(434, 420);
      this.pictureSource.TabIndex = 5;
      this.pictureSource.TabStop = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(906, 485);
      this.Controls.Add(this.labelElapsed);
      this.Controls.Add(this.checkBoxWhite);
      this.Controls.Add(this.labelPen);
      this.Controls.Add(this.numericPen);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.buttonSaveMask);
      this.Controls.Add(this.buttonLoadMask);
      this.Controls.Add(this.buttonRecompute);
      this.Controls.Add(this.tableImages);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.buttonSave);
      this.MinimumSize = new System.Drawing.Size(880, 300);
      this.Name = "Form1";
      this.Text = "085 segmentation";
      this.tableImages.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericPen)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private GUIPictureBox pictureSource;
    private System.Windows.Forms.PictureBox pictureTarget;
    private System.Windows.Forms.TableLayoutPanel tableImages;
    public System.Windows.Forms.Button buttonOpen;
    public System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonRecompute;
    private System.Windows.Forms.Button buttonLoadMask;
    private System.Windows.Forms.Button buttonSaveMask;
    private System.Windows.Forms.Button buttonReset;
    private System.Windows.Forms.NumericUpDown numericPen;
    private System.Windows.Forms.Label labelPen;
    private System.Windows.Forms.CheckBox checkBoxWhite;
    private System.Windows.Forms.Label labelElapsed;
  }
}
