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
      this.pictureSource = new _085segmentation.GUIPictureBox();
      this.tableImages.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).BeginInit();
      this.SuspendLayout();
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(12, 450);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(106, 23);
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
      // buttonRecompute
      // 
      this.buttonRecompute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRecompute.Location = new System.Drawing.Point(137, 450);
      this.buttonRecompute.Name = "buttonRecompute";
      this.buttonRecompute.Size = new System.Drawing.Size(104, 23);
      this.buttonRecompute.TabIndex = 3;
      this.buttonRecompute.Text = "Recompute";
      this.buttonRecompute.UseVisualStyleBackColor = true;
      this.buttonRecompute.Click += new System.EventHandler(this.buttonRecompute_Click);
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
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(821, 485);
      this.Controls.Add(this.buttonRecompute);
      this.Controls.Add(this.tableImages);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.buttonSave);
      this.MinimumSize = new System.Drawing.Size(620, 200);
      this.Name = "Form1";
      this.Text = "085 segmentation";
      this.tableImages.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private GUIPictureBox pictureSource;
    private System.Windows.Forms.PictureBox pictureTarget;
    private System.Windows.Forms.TableLayoutPanel tableImages;
    public System.Windows.Forms.Button buttonOpen;
    public System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonRecompute;
  }
}
