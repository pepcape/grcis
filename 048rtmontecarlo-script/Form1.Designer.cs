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
      if ( disposing && ( components != null ) )
      {
        components.Dispose ();
      }
      base.Dispose ( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.components = new System.ComponentModel.Container();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.buttonSave = new System.Windows.Forms.Button();
      this.savePointCloudButton = new System.Windows.Forms.Button();
      this.AdditionalViewsButton = new System.Windows.Forms.Button();
      this.RayVisualiserButton = new System.Windows.Forms.Button();
      this.RenderClientsButton = new System.Windows.Forms.Button();
      this.ResetButton = new System.Windows.Forms.Button();
      this.NextImageButton = new System.Windows.Forms.Button();
      this.PreviousImageButton = new System.Windows.Forms.Button();
      this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
      this.buttonRender = new System.Windows.Forms.Button();
      this.buttonStop = new System.Windows.Forms.Button();
      this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
      this.buttonRes = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.NumericSupersampling = new System.Windows.Forms.NumericUpDown();
      this.CheckJitter = new System.Windows.Forms.CheckBox();
      this.checkShadows = new System.Windows.Forms.CheckBox();
      this.checkReflections = new System.Windows.Forms.CheckBox();
      this.checkRefractions = new System.Windows.Forms.CheckBox();
      this.CheckMultithreading = new System.Windows.Forms.CheckBox();
      this.pointCloudCheckBox = new System.Windows.Forms.CheckBox();
      this.collectDataCheckBox = new System.Windows.Forms.CheckBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.TextParam = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.ComboScene = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.labelSample = new System.Windows.Forms.Label();
      this.notificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.flowLayoutPanel3.SuspendLayout();
      this.flowLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.NumericSupersampling)).BeginInit();
      this.panel1.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(909, 632);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox1.InitialImage = null;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(909, 519);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 51;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
      this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
      this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
      this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
      this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel3, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 603);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(909, 29);
      this.tableLayoutPanel2.TabIndex = 5;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Controls.Add(this.buttonSave);
      this.flowLayoutPanel1.Controls.Add(this.savePointCloudButton);
      this.flowLayoutPanel1.Controls.Add(this.AdditionalViewsButton);
      this.flowLayoutPanel1.Controls.Add(this.RayVisualiserButton);
      this.flowLayoutPanel1.Controls.Add(this.RenderClientsButton);
      this.flowLayoutPanel1.Controls.Add(this.ResetButton);
      this.flowLayoutPanel1.Controls.Add(this.NextImageButton);
      this.flowLayoutPanel1.Controls.Add(this.PreviousImageButton);
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(181, 0);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(728, 29);
      this.flowLayoutPanel1.TabIndex = 4;
      this.flowLayoutPanel1.WrapContents = false;
      // 
      // buttonSave
      // 
      this.buttonSave.Enabled = false;
      this.buttonSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonSave.Image = global::_048rtmontecarlo.Properties.Resources.Save_16x;
      this.buttonSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.buttonSave.Location = new System.Drawing.Point(641, 3);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(84, 23);
      this.buttonSave.TabIndex = 4;
      this.buttonSave.Text = "Save image";
      this.buttonSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // savePointCloudButton
      // 
      this.savePointCloudButton.Enabled = false;
      this.savePointCloudButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.savePointCloudButton.Image = global::_048rtmontecarlo.Properties.Resources.Save_16x;
      this.savePointCloudButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.savePointCloudButton.Location = new System.Drawing.Point(526, 3);
      this.savePointCloudButton.Name = "savePointCloudButton";
      this.savePointCloudButton.Size = new System.Drawing.Size(109, 23);
      this.savePointCloudButton.TabIndex = 8;
      this.savePointCloudButton.Text = "Save point cloud";
      this.savePointCloudButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.savePointCloudButton.UseVisualStyleBackColor = true;
      this.savePointCloudButton.Click += new System.EventHandler(this.SavePointCloudButton_Click);
      // 
      // AdditionalViewsButton
      // 
      this.AdditionalViewsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.AdditionalViewsButton.Location = new System.Drawing.Point(413, 3);
      this.AdditionalViewsButton.Name = "AdditionalViewsButton";
      this.AdditionalViewsButton.Size = new System.Drawing.Size(107, 23);
      this.AdditionalViewsButton.TabIndex = 5;
      this.AdditionalViewsButton.Text = "Additional Views";
      this.AdditionalViewsButton.UseVisualStyleBackColor = true;
      this.AdditionalViewsButton.Click += new System.EventHandler(this.AdditionalViewsButton_Click);
      // 
      // RayVisualiserButton
      // 
      this.RayVisualiserButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RayVisualiserButton.Location = new System.Drawing.Point(315, 3);
      this.RayVisualiserButton.Name = "RayVisualiserButton";
      this.RayVisualiserButton.Size = new System.Drawing.Size(92, 23);
      this.RayVisualiserButton.TabIndex = 6;
      this.RayVisualiserButton.Text = "Ray Visualizer";
      this.RayVisualiserButton.UseVisualStyleBackColor = true;
      this.RayVisualiserButton.Click += new System.EventHandler(this.RayVisualiserButton_Click);
      // 
      // RenderClientsButton
      // 
      this.RenderClientsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RenderClientsButton.Location = new System.Drawing.Point(210, 3);
      this.RenderClientsButton.Name = "RenderClientsButton";
      this.RenderClientsButton.Size = new System.Drawing.Size(99, 23);
      this.RenderClientsButton.TabIndex = 7;
      this.RenderClientsButton.Text = "Render Clients";
      this.RenderClientsButton.UseVisualStyleBackColor = true;
      this.RenderClientsButton.Click += new System.EventHandler(this.addRenderClientToolStripMenuItem_Click);
      // 
      // ResetButton
      // 
      this.ResetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ResetButton.Location = new System.Drawing.Point(161, 3);
      this.ResetButton.Name = "ResetButton";
      this.ResetButton.Size = new System.Drawing.Size(43, 23);
      this.ResetButton.TabIndex = 9;
      this.ResetButton.Text = "Reset";
      this.ResetButton.UseVisualStyleBackColor = true;
      this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
      // 
      // NextImageButton
      // 
      this.NextImageButton.Enabled = false;
      this.NextImageButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.NextImageButton.Location = new System.Drawing.Point(101, 3);
      this.NextImageButton.Name = "NextImageButton";
      this.NextImageButton.Size = new System.Drawing.Size(54, 23);
      this.NextImageButton.TabIndex = 11;
      this.NextImageButton.Text = ">>";
      this.NextImageButton.UseVisualStyleBackColor = true;
      this.NextImageButton.Click += new System.EventHandler(this.NextImageButton_Click);
      // 
      // PreviousImageButton
      // 
      this.PreviousImageButton.Enabled = false;
      this.PreviousImageButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PreviousImageButton.Location = new System.Drawing.Point(41, 3);
      this.PreviousImageButton.Name = "PreviousImageButton";
      this.PreviousImageButton.Size = new System.Drawing.Size(54, 23);
      this.PreviousImageButton.TabIndex = 10;
      this.PreviousImageButton.Text = "<<";
      this.PreviousImageButton.UseVisualStyleBackColor = true;
      this.PreviousImageButton.Click += new System.EventHandler(this.PreviousImageButton_Click);
      // 
      // flowLayoutPanel3
      // 
      this.flowLayoutPanel3.Controls.Add(this.buttonRender);
      this.flowLayoutPanel3.Controls.Add(this.buttonStop);
      this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel3.Name = "flowLayoutPanel3";
      this.flowLayoutPanel3.Size = new System.Drawing.Size(181, 29);
      this.flowLayoutPanel3.TabIndex = 5;
      this.flowLayoutPanel3.WrapContents = false;
      // 
      // buttonRender
      // 
      this.buttonRender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonRender.Location = new System.Drawing.Point(3, 3);
      this.buttonRender.Name = "buttonRender";
      this.buttonRender.Size = new System.Drawing.Size(108, 23);
      this.buttonRender.TabIndex = 8;
      this.buttonRender.Text = "Render";
      this.buttonRender.UseVisualStyleBackColor = true;
      this.buttonRender.Click += new System.EventHandler(this.buttonRender_Click);
      // 
      // buttonStop
      // 
      this.buttonStop.Enabled = false;
      this.buttonStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonStop.Location = new System.Drawing.Point(117, 3);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(51, 23);
      this.buttonStop.TabIndex = 9;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // flowLayoutPanel2
      // 
      this.flowLayoutPanel2.Controls.Add(this.buttonRes);
      this.flowLayoutPanel2.Controls.Add(this.label2);
      this.flowLayoutPanel2.Controls.Add(this.NumericSupersampling);
      this.flowLayoutPanel2.Controls.Add(this.CheckJitter);
      this.flowLayoutPanel2.Controls.Add(this.checkShadows);
      this.flowLayoutPanel2.Controls.Add(this.checkReflections);
      this.flowLayoutPanel2.Controls.Add(this.checkRefractions);
      this.flowLayoutPanel2.Controls.Add(this.CheckMultithreading);
      this.flowLayoutPanel2.Controls.Add(this.pointCloudCheckBox);
      this.flowLayoutPanel2.Controls.Add(this.collectDataCheckBox);
      this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 554);
      this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel2.Name = "flowLayoutPanel2";
      this.flowLayoutPanel2.Size = new System.Drawing.Size(909, 29);
      this.flowLayoutPanel2.TabIndex = 1;
      // 
      // buttonRes
      // 
      this.buttonRes.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.buttonRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonRes.Location = new System.Drawing.Point(3, 3);
      this.buttonRes.Name = "buttonRes";
      this.buttonRes.Size = new System.Drawing.Size(83, 23);
      this.buttonRes.TabIndex = 43;
      this.buttonRes.Text = "Resolution";
      this.buttonRes.UseVisualStyleBackColor = true;
      this.buttonRes.Click += new System.EventHandler(this.buttonRes_Click);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(92, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(79, 13);
      this.label2.TabIndex = 44;
      this.label2.Text = "Supersampling:";
      // 
      // NumericSupersampling
      // 
      this.NumericSupersampling.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.NumericSupersampling.Location = new System.Drawing.Point(177, 4);
      this.NumericSupersampling.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.NumericSupersampling.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.NumericSupersampling.Name = "NumericSupersampling";
      this.NumericSupersampling.Size = new System.Drawing.Size(52, 20);
      this.NumericSupersampling.TabIndex = 45;
      this.NumericSupersampling.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // CheckJitter
      // 
      this.CheckJitter.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.CheckJitter.AutoSize = true;
      this.CheckJitter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.CheckJitter.Checked = true;
      this.CheckJitter.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CheckJitter.Location = new System.Drawing.Point(235, 6);
      this.CheckJitter.Name = "CheckJitter";
      this.CheckJitter.Size = new System.Drawing.Size(59, 17);
      this.CheckJitter.TabIndex = 49;
      this.CheckJitter.Text = "jittering";
      this.CheckJitter.UseVisualStyleBackColor = true;
      // 
      // checkShadows
      // 
      this.checkShadows.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.checkShadows.AutoSize = true;
      this.checkShadows.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkShadows.Checked = true;
      this.checkShadows.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkShadows.Location = new System.Drawing.Point(300, 6);
      this.checkShadows.Name = "checkShadows";
      this.checkShadows.Size = new System.Drawing.Size(68, 17);
      this.checkShadows.TabIndex = 46;
      this.checkShadows.Text = "shadows";
      this.checkShadows.UseVisualStyleBackColor = true;
      // 
      // checkReflections
      // 
      this.checkReflections.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.checkReflections.AutoSize = true;
      this.checkReflections.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkReflections.Checked = true;
      this.checkReflections.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkReflections.Location = new System.Drawing.Point(374, 6);
      this.checkReflections.Name = "checkReflections";
      this.checkReflections.Size = new System.Drawing.Size(74, 17);
      this.checkReflections.TabIndex = 47;
      this.checkReflections.Text = "reflections";
      this.checkReflections.UseVisualStyleBackColor = true;
      // 
      // checkRefractions
      // 
      this.checkRefractions.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.checkRefractions.AutoSize = true;
      this.checkRefractions.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkRefractions.Checked = true;
      this.checkRefractions.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkRefractions.Location = new System.Drawing.Point(454, 6);
      this.checkRefractions.Name = "checkRefractions";
      this.checkRefractions.Size = new System.Drawing.Size(75, 17);
      this.checkRefractions.TabIndex = 48;
      this.checkRefractions.Text = "refractions";
      this.checkRefractions.UseVisualStyleBackColor = true;
      // 
      // CheckMultithreading
      // 
      this.CheckMultithreading.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.CheckMultithreading.AutoSize = true;
      this.CheckMultithreading.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.CheckMultithreading.Location = new System.Drawing.Point(535, 6);
      this.CheckMultithreading.Name = "CheckMultithreading";
      this.CheckMultithreading.Size = new System.Drawing.Size(94, 17);
      this.CheckMultithreading.TabIndex = 50;
      this.CheckMultithreading.Text = "multi-threading";
      this.CheckMultithreading.UseVisualStyleBackColor = true;
      // 
      // pointCloudCheckBox
      // 
      this.pointCloudCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.pointCloudCheckBox.AutoSize = true;
      this.pointCloudCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.pointCloudCheckBox.Location = new System.Drawing.Point(635, 6);
      this.pointCloudCheckBox.Name = "pointCloudCheckBox";
      this.pointCloudCheckBox.Size = new System.Drawing.Size(78, 17);
      this.pointCloudCheckBox.TabIndex = 51;
      this.pointCloudCheckBox.Text = "point cloud";
      this.pointCloudCheckBox.UseVisualStyleBackColor = true;
      this.pointCloudCheckBox.CheckedChanged += new System.EventHandler(this.pointCloudCheckBox_CheckedChanged);
      // 
      // collectDataCheckBox
      // 
      this.collectDataCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.collectDataCheckBox.AutoSize = true;
      this.collectDataCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.collectDataCheckBox.Checked = true;
      this.collectDataCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.collectDataCheckBox.Location = new System.Drawing.Point(719, 6);
      this.collectDataCheckBox.Name = "collectDataCheckBox";
      this.collectDataCheckBox.Size = new System.Drawing.Size(129, 17);
      this.collectDataCheckBox.TabIndex = 52;
      this.collectDataCheckBox.Text = "collect additional data";
      this.collectDataCheckBox.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.TextParam);
      this.panel1.Controls.Add(this.label3);
      this.panel1.Controls.Add(this.ComboScene);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(3, 522);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(903, 29);
      this.panel1.TabIndex = 3;
      // 
      // TextParam
      // 
      this.TextParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TextParam.Location = new System.Drawing.Point(272, 5);
      this.TextParam.Name = "TextParam";
      this.TextParam.Size = new System.Drawing.Size(628, 20);
      this.TextParam.TabIndex = 49;
      this.TextParam.MouseHover += new System.EventHandler(this.TextParam_MouseHover);
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(211, 9);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(63, 13);
      this.label3.TabIndex = 48;
      this.label3.Text = "Parameters:";
      // 
      // ComboScene
      // 
      this.ComboScene.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.ComboScene.FormattingEnabled = true;
      this.ComboScene.Location = new System.Drawing.Point(50, 5);
      this.ComboScene.Name = "ComboScene";
      this.ComboScene.Size = new System.Drawing.Size(155, 21);
      this.ComboScene.TabIndex = 47;
      this.ComboScene.SelectedIndexChanged += new System.EventHandler(this.comboScene_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(41, 13);
      this.label1.TabIndex = 46;
      this.label1.Text = "Scene:";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.tableLayoutPanel3.Controls.Add(this.labelElapsed, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.labelSample, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 583);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(909, 20);
      this.tableLayoutPanel3.TabIndex = 4;
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point(3, 3);
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size(48, 13);
      this.labelElapsed.TabIndex = 24;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelSample
      // 
      this.labelSample.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labelSample.AutoSize = true;
      this.labelSample.Location = new System.Drawing.Point(609, 3);
      this.labelSample.Name = "labelSample";
      this.labelSample.Size = new System.Drawing.Size(45, 13);
      this.labelSample.TabIndex = 23;
      this.labelSample.Text = "Sample:";
      // 
      // notificationIcon
      // 
      this.notificationIcon.Text = "048 Monte Carlo RT script";
      this.notificationIcon.Visible = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(909, 632);
      this.Controls.Add(this.tableLayoutPanel1);
      this.MinimumSize = new System.Drawing.Size(680, 300);
      this.Name = "Form1";
      this.Text = "048 Monte Carlo RT script";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel3.ResumeLayout(false);
      this.flowLayoutPanel2.ResumeLayout(false);
      this.flowLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.NumericSupersampling)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    private System.Windows.Forms.Button buttonRes;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox checkShadows;
    private System.Windows.Forms.CheckBox checkReflections;
    private System.Windows.Forms.CheckBox checkRefractions;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label labelSample;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button RenderClientsButton;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
    private System.Windows.Forms.Button buttonRender;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button AdditionalViewsButton;
    public System.Windows.Forms.CheckBox pointCloudCheckBox;
    private System.Windows.Forms.NotifyIcon notificationIcon;
    public System.Windows.Forms.Button RayVisualiserButton;
    private System.Windows.Forms.Button ResetButton;
    public System.Windows.Forms.CheckBox collectDataCheckBox;
    public System.Windows.Forms.Button savePointCloudButton;
    private System.Windows.Forms.Button NextImageButton;
    private System.Windows.Forms.Button PreviousImageButton;
  }
}
