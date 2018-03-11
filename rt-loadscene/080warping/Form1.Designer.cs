namespace _080warping
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
      this.numericParam = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.tableImages = new System.Windows.Forms.TableLayoutPanel();
      this.pictureSource = new _080warping.GUIPictureBox();
      this.pictureTarget = new _080warping.GUIPictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).BeginInit();
      this.tableImages.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(12, 450);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(130, 23);
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load image";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(679, 450);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(130, 23);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // numericParam
      // 
      this.numericParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericParam.Location = new System.Drawing.Point(241, 453);
      this.numericParam.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
      this.numericParam.Name = "numericParam";
      this.numericParam.Size = new System.Drawing.Size(62, 20);
      this.numericParam.TabIndex = 3;
      this.numericParam.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
      this.numericParam.ValueChanged += new System.EventHandler(this.numericParam_ValueChanged);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.Location = new System.Drawing.Point(168, 455);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(58, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Mesh size:";
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
      this.tableImages.Size = new System.Drawing.Size(795, 426);
      this.tableImages.TabIndex = 0;
      // 
      // pictureSource
      // 
      this.pictureSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureSource.Location = new System.Drawing.Point(3, 3);
      this.pictureSource.Name = "pictureSource";
      this.pictureSource.Size = new System.Drawing.Size(391, 420);
      this.pictureSource.TabIndex = 5;
      this.pictureSource.TabStop = false;
      // 
      // pictureTarget
      // 
      this.pictureTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureTarget.Location = new System.Drawing.Point(400, 3);
      this.pictureTarget.Name = "pictureTarget";
      this.pictureTarget.Size = new System.Drawing.Size(392, 420);
      this.pictureTarget.TabIndex = 6;
      this.pictureTarget.TabStop = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(821, 485);
      this.Controls.Add(this.tableImages);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.numericParam);
      this.Controls.Add(this.buttonSave);
      this.MinimumSize = new System.Drawing.Size(620, 200);
      this.Name = "Form1";
      this.Text = "080 trimesh warping";
      ((System.ComponentModel.ISupportInitialize)(this.numericParam)).EndInit();
      this.tableImages.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private GUIPictureBox pictureSource;
    private GUIPictureBox pictureTarget;
    private System.Windows.Forms.TableLayoutPanel tableImages;
    public System.Windows.Forms.Button buttonOpen;
    public System.Windows.Forms.Button buttonSave;
    public System.Windows.Forms.NumericUpDown numericParam;
    private System.Windows.Forms.Label label1;
  }
}
