namespace _057scene
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
      this.buttonOpen = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.numericAzimuth = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.buttonRedraw = new System.Windows.Forms.Button();
      this.labelFaces = new System.Windows.Forms.Label();
      this.checkNormals = new System.Windows.Forms.CheckBox();
      this.buttonSaveOBJ = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.numericElevation = new System.Windows.Forms.NumericUpDown();
      this.buttonGenerate = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.checkPerspective = new System.Windows.Forms.CheckBox();
      this.textParam = new System.Windows.Forms.TextBox();
      this.checkMulti = new System.Windows.Forms.CheckBox();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericElevation)).BeginInit();
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
      this.panel1.Size = new System.Drawing.Size(680, 350);
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(680, 350);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      // 
      // buttonOpen
      // 
      this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonOpen.Location = new System.Drawing.Point(13, 377);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(108, 23);
      this.buttonOpen.TabIndex = 1;
      this.buttonOpen.Text = "Load scene";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point(586, 412);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(107, 23);
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // numericAzimuth
      // 
      this.numericAzimuth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.numericAzimuth.DecimalPlaces = 1;
      this.numericAzimuth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericAzimuth.Location = new System.Drawing.Point(368, 378);
      this.numericAzimuth.Maximum = new decimal(new int[] {
            720,
            0,
            0,
            0});
      this.numericAzimuth.Minimum = new decimal(new int[] {
            720,
            0,
            0,
            -2147483648});
      this.numericAzimuth.Name = "numericAzimuth";
      this.numericAzimuth.Size = new System.Drawing.Size(62, 20);
      this.numericAzimuth.TabIndex = 3;
      this.numericAzimuth.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(306, 418);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(54, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Elevation:";
      // 
      // buttonRedraw
      // 
      this.buttonRedraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonRedraw.Location = new System.Drawing.Point(446, 412);
      this.buttonRedraw.Name = "buttonRedraw";
      this.buttonRedraw.Size = new System.Drawing.Size(125, 23);
      this.buttonRedraw.TabIndex = 5;
      this.buttonRedraw.Text = "Redraw";
      this.buttonRedraw.UseVisualStyleBackColor = true;
      this.buttonRedraw.Click += new System.EventHandler(this.buttonRedraw_Click);
      // 
      // labelFaces
      // 
      this.labelFaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelFaces.AutoSize = true;
      this.labelFaces.Location = new System.Drawing.Point(133, 383);
      this.labelFaces.Name = "labelFaces";
      this.labelFaces.Size = new System.Drawing.Size(33, 13);
      this.labelFaces.TabIndex = 6;
      this.labelFaces.Text = "faces";
      // 
      // checkNormals
      // 
      this.checkNormals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkNormals.AutoSize = true;
      this.checkNormals.Location = new System.Drawing.Point(513, 381);
      this.checkNormals.Name = "checkNormals";
      this.checkNormals.Size = new System.Drawing.Size(62, 17);
      this.checkNormals.TabIndex = 7;
      this.checkNormals.Text = "normals";
      this.checkNormals.UseVisualStyleBackColor = true;
      // 
      // buttonSaveOBJ
      // 
      this.buttonSaveOBJ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSaveOBJ.Location = new System.Drawing.Point(586, 377);
      this.buttonSaveOBJ.Name = "buttonSaveOBJ";
      this.buttonSaveOBJ.Size = new System.Drawing.Size(107, 23);
      this.buttonSaveOBJ.TabIndex = 8;
      this.buttonSaveOBJ.Text = "Save OBJ";
      this.buttonSaveOBJ.UseVisualStyleBackColor = true;
      this.buttonSaveOBJ.Click += new System.EventHandler(this.buttonSaveOBJ_Click);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(306, 380);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(47, 13);
      this.label2.TabIndex = 9;
      this.label2.Text = "Azimuth:";
      // 
      // numericElevation
      // 
      this.numericElevation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.numericElevation.DecimalPlaces = 1;
      this.numericElevation.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericElevation.Location = new System.Drawing.Point(368, 414);
      this.numericElevation.Maximum = new decimal(new int[] {
            89,
            0,
            0,
            0});
      this.numericElevation.Minimum = new decimal(new int[] {
            89,
            0,
            0,
            -2147483648});
      this.numericElevation.Name = "numericElevation";
      this.numericElevation.Size = new System.Drawing.Size(62, 20);
      this.numericElevation.TabIndex = 10;
      this.numericElevation.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      // 
      // buttonGenerate
      // 
      this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonGenerate.Location = new System.Drawing.Point(13, 410);
      this.buttonGenerate.Name = "buttonGenerate";
      this.buttonGenerate.Size = new System.Drawing.Size(108, 23);
      this.buttonGenerate.TabIndex = 11;
      this.buttonGenerate.Text = "Generate";
      this.buttonGenerate.UseVisualStyleBackColor = true;
      this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(133, 416);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 13;
      this.label3.Text = "Param:";
      // 
      // checkPerspective
      // 
      this.checkPerspective.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.checkPerspective.AutoSize = true;
      this.checkPerspective.Checked = true;
      this.checkPerspective.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkPerspective.Location = new System.Drawing.Point(450, 380);
      this.checkPerspective.Name = "checkPerspective";
      this.checkPerspective.Size = new System.Drawing.Size(55, 17);
      this.checkPerspective.TabIndex = 14;
      this.checkPerspective.Text = "persp.";
      this.checkPerspective.UseVisualStyleBackColor = true;
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point(180, 412);
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size(100, 20);
      this.textParam.TabIndex = 15;
      // 
      // checkMulti
      // 
      this.checkMulti.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkMulti.AutoSize = true;
      this.checkMulti.Location = new System.Drawing.Point(265, 383);
      this.checkMulti.Name = "checkMulti";
      this.checkMulti.Size = new System.Drawing.Size(15, 14);
      this.checkMulti.TabIndex = 16;
      this.checkMulti.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(712, 446);
      this.Controls.Add(this.checkMulti);
      this.Controls.Add(this.textParam);
      this.Controls.Add(this.checkPerspective);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.buttonGenerate);
      this.Controls.Add(this.numericElevation);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.buttonSaveOBJ);
      this.Controls.Add(this.checkNormals);
      this.Controls.Add(this.labelFaces);
      this.Controls.Add(this.buttonRedraw);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.numericAzimuth);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.panel1);
      this.MinimumSize = new System.Drawing.Size(710, 200);
      this.Name = "Form1";
      this.Text = "057 scene";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericAzimuth)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericElevation)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonOpen;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.NumericUpDown numericAzimuth;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button buttonRedraw;
    private System.Windows.Forms.Label labelFaces;
    private System.Windows.Forms.CheckBox checkNormals;
    private System.Windows.Forms.Button buttonSaveOBJ;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericElevation;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox checkPerspective;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.CheckBox checkMulti;
  }
}

