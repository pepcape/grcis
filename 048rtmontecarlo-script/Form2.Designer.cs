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
			this.DepthMapPanel = new System.Windows.Forms.Panel();
			this.DepthMap_Coordinates = new System.Windows.Forms.Label();
			this.SaveDepthMapButton = new System.Windows.Forms.Button();
			this.RenderDepthMapButton = new System.Windows.Forms.Button();
			this.DepthMapPictureBox = new System.Windows.Forms.PictureBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.RenderPrimaryRaysMapButton = new System.Windows.Forms.Button();
			this.PrimaryRaysMapPanel = new System.Windows.Forms.Panel();
			this.SavePrimaryRaysMapButton = new System.Windows.Forms.Button();
			this.AveragePrimaryRaysCount = new System.Windows.Forms.Label();
			this.PrimaryRaysMapPictureBox = new System.Windows.Forms.PictureBox();
			this.TotalPrimaryRaysCount = new System.Windows.Forms.Label();
			this.PrimaryRaysMapCoordinates = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.RenderAllRaysMapButtona = new System.Windows.Forms.Button();
			this.AllRaysMapPanel = new System.Windows.Forms.Panel();
			this.SaveAllRaysMapButton = new System.Windows.Forms.Button();
			this.AverageAllRaysCount = new System.Windows.Forms.Label();
			this.AllRaysMapPictureBox = new System.Windows.Forms.PictureBox();
			this.TotalAllRaysCount = new System.Windows.Forms.Label();
			this.AllRaysMapCoordinates = new System.Windows.Forms.Label();
			this.NormalMap = new System.Windows.Forms.TabPage();
			this.RenderNormalMapRelativeButton = new System.Windows.Forms.Button();
			this.NormalMapRelativePanel = new System.Windows.Forms.Panel();
			this.SaveNormalMapRelativeButton = new System.Windows.Forms.Button();
			this.NormalMapRelativePictureBox = new System.Windows.Forms.PictureBox();
			this.NormalMapRelativeCoordinates = new System.Windows.Forms.Label();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.RenderNormalMapAbsoluteButton = new System.Windows.Forms.Button();
			this.NormalMapAbsolutePanel = new System.Windows.Forms.Panel();
			this.SaveNormalMapAbsoluteButton = new System.Windows.Forms.Button();
			this.NormalMapAbsolutePictureBox = new System.Windows.Forms.PictureBox();
			this.NormalMapAbsoluteCoordinates = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.DepthMapPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).BeginInit();
			this.tabPage2.SuspendLayout();
			this.PrimaryRaysMapPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PrimaryRaysMapPictureBox)).BeginInit();
			this.tabPage3.SuspendLayout();
			this.AllRaysMapPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.AllRaysMapPictureBox)).BeginInit();
			this.NormalMap.SuspendLayout();
			this.NormalMapRelativePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NormalMapRelativePictureBox)).BeginInit();
			this.tabPage5.SuspendLayout();
			this.NormalMapAbsolutePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NormalMapAbsolutePictureBox)).BeginInit();
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
			this.tabControl1.Controls.Add(this.NormalMap);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(809, 537);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.Tag = "";
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.DepthMapPanel);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(801, 511);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Depth Map";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// DepthMapPanel
			// 
			this.DepthMapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DepthMapPanel.AutoScroll = true;
			this.DepthMapPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.DepthMapPanel.Controls.Add(this.DepthMap_Coordinates);
			this.DepthMapPanel.Controls.Add(this.SaveDepthMapButton);
			this.DepthMapPanel.Controls.Add(this.RenderDepthMapButton);
			this.DepthMapPanel.Controls.Add(this.DepthMapPictureBox);
			this.DepthMapPanel.Location = new System.Drawing.Point(0, 0);
			this.DepthMapPanel.Name = "DepthMapPanel";
			this.DepthMapPanel.Size = new System.Drawing.Size(801, 511);
			this.DepthMapPanel.TabIndex = 3;
			this.DepthMapPanel.Tag = "DepthMap";
			// 
			// DepthMap_Coordinates
			// 
			this.DepthMap_Coordinates.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.DepthMap_Coordinates.AutoSize = true;
			this.DepthMap_Coordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DepthMap_Coordinates.Location = new System.Drawing.Point(3, 82);
			this.DepthMap_Coordinates.Name = "DepthMap_Coordinates";
			this.DepthMap_Coordinates.Size = new System.Drawing.Size(61, 60);
			this.DepthMap_Coordinates.TabIndex = 7;
			this.DepthMap_Coordinates.Text = "X: \r\nY: \r\nDepth: \r\n";
			// 
			// SaveDepthMapButton
			// 
			this.SaveDepthMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SaveDepthMapButton.Enabled = false;
			this.SaveDepthMapButton.Location = new System.Drawing.Point(7, 445);
			this.SaveDepthMapButton.Name = "SaveDepthMapButton";
			this.SaveDepthMapButton.Size = new System.Drawing.Size(97, 41);
			this.SaveDepthMapButton.TabIndex = 5;
			this.SaveDepthMapButton.Text = "Save Image";
			this.SaveDepthMapButton.UseVisualStyleBackColor = true;
			this.SaveDepthMapButton.Click += new System.EventHandler(this.SaveMapButton_Click);
			// 
			// RenderDepthMapButton
			// 
			this.RenderDepthMapButton.Location = new System.Drawing.Point(7, 6);
			this.RenderDepthMapButton.Name = "RenderDepthMapButton";
			this.RenderDepthMapButton.Size = new System.Drawing.Size(97, 41);
			this.RenderDepthMapButton.TabIndex = 4;
			this.RenderDepthMapButton.Tag = "depthMap";
			this.RenderDepthMapButton.Text = "Render";
			this.RenderDepthMapButton.UseVisualStyleBackColor = true;
			this.RenderDepthMapButton.Click += new System.EventHandler(this.RenderMapButton_Click);
			// 
			// DepthMapPictureBox
			// 
			this.DepthMapPictureBox.Location = new System.Drawing.Point(110, 6);
			this.DepthMapPictureBox.Name = "DepthMapPictureBox";
			this.DepthMapPictureBox.Size = new System.Drawing.Size(680, 480);
			this.DepthMapPictureBox.TabIndex = 3;
			this.DepthMapPictureBox.TabStop = false;
			this.DepthMapPictureBox.Tag = "depthMap";
			this.DepthMapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DepthMapPictureBox_MouseDownAndMouseMove);
			this.DepthMapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DepthMapPictureBox_MouseDownAndMouseMove);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.RenderPrimaryRaysMapButton);
			this.tabPage2.Controls.Add(this.PrimaryRaysMapPanel);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(801, 511);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Primary Rays Map";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// RenderPrimaryRaysMapButton
			// 
			this.RenderPrimaryRaysMapButton.Location = new System.Drawing.Point(7, 6);
			this.RenderPrimaryRaysMapButton.Name = "RenderPrimaryRaysMapButton";
			this.RenderPrimaryRaysMapButton.Size = new System.Drawing.Size(97, 41);
			this.RenderPrimaryRaysMapButton.TabIndex = 8;
			this.RenderPrimaryRaysMapButton.Tag = "primaryRaysMap";
			this.RenderPrimaryRaysMapButton.Text = "Render";
			this.RenderPrimaryRaysMapButton.UseVisualStyleBackColor = true;
			this.RenderPrimaryRaysMapButton.Click += new System.EventHandler(this.RenderMapButton_Click);
			// 
			// PrimaryRaysMapPanel
			// 
			this.PrimaryRaysMapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PrimaryRaysMapPanel.AutoScroll = true;
			this.PrimaryRaysMapPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.PrimaryRaysMapPanel.Controls.Add(this.SavePrimaryRaysMapButton);
			this.PrimaryRaysMapPanel.Controls.Add(this.AveragePrimaryRaysCount);
			this.PrimaryRaysMapPanel.Controls.Add(this.PrimaryRaysMapPictureBox);
			this.PrimaryRaysMapPanel.Controls.Add(this.TotalPrimaryRaysCount);
			this.PrimaryRaysMapPanel.Controls.Add(this.PrimaryRaysMapCoordinates);
			this.PrimaryRaysMapPanel.Location = new System.Drawing.Point(0, 0);
			this.PrimaryRaysMapPanel.Name = "PrimaryRaysMapPanel";
			this.PrimaryRaysMapPanel.Size = new System.Drawing.Size(801, 511);
			this.PrimaryRaysMapPanel.TabIndex = 6;
			this.PrimaryRaysMapPanel.Tag = "PrimaryRaysMap";
			// 
			// SavePrimaryRaysMapButton
			// 
			this.SavePrimaryRaysMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SavePrimaryRaysMapButton.Enabled = false;
			this.SavePrimaryRaysMapButton.Location = new System.Drawing.Point(7, 445);
			this.SavePrimaryRaysMapButton.Name = "SavePrimaryRaysMapButton";
			this.SavePrimaryRaysMapButton.Size = new System.Drawing.Size(97, 41);
			this.SavePrimaryRaysMapButton.TabIndex = 9;
			this.SavePrimaryRaysMapButton.Text = "Save Image";
			this.SavePrimaryRaysMapButton.UseVisualStyleBackColor = true;
			this.SavePrimaryRaysMapButton.Click += new System.EventHandler(this.SaveMapButton_Click);
			// 
			// AveragePrimaryRaysCount
			// 
			this.AveragePrimaryRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.AveragePrimaryRaysCount.AutoSize = true;
			this.AveragePrimaryRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AveragePrimaryRaysCount.Location = new System.Drawing.Point(6, 292);
			this.AveragePrimaryRaysCount.Name = "AveragePrimaryRaysCount";
			this.AveragePrimaryRaysCount.Size = new System.Drawing.Size(82, 80);
			this.AveragePrimaryRaysCount.TabIndex = 11;
			this.AveragePrimaryRaysCount.Text = "Average\r\nprimary\r\nrays count\r\nper pixel:\r\n";
			// 
			// PrimaryRaysMapPictureBox
			// 
			this.PrimaryRaysMapPictureBox.Location = new System.Drawing.Point(110, 6);
			this.PrimaryRaysMapPictureBox.Name = "PrimaryRaysMapPictureBox";
			this.PrimaryRaysMapPictureBox.Size = new System.Drawing.Size(680, 480);
			this.PrimaryRaysMapPictureBox.TabIndex = 7;
			this.PrimaryRaysMapPictureBox.TabStop = false;
			this.PrimaryRaysMapPictureBox.Tag = "primaryRaysMap";
			this.PrimaryRaysMapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
			this.PrimaryRaysMapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
			// 
			// TotalPrimaryRaysCount
			// 
			this.TotalPrimaryRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.TotalPrimaryRaysCount.AutoSize = true;
			this.TotalPrimaryRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TotalPrimaryRaysCount.Location = new System.Drawing.Point(6, 179);
			this.TotalPrimaryRaysCount.Name = "TotalPrimaryRaysCount";
			this.TotalPrimaryRaysCount.Size = new System.Drawing.Size(103, 40);
			this.TotalPrimaryRaysCount.TabIndex = 10;
			this.TotalPrimaryRaysCount.Text = "Total primary \r\nrays count:";
			// 
			// PrimaryRaysMapCoordinates
			// 
			this.PrimaryRaysMapCoordinates.AutoSize = true;
			this.PrimaryRaysMapCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PrimaryRaysMapCoordinates.Location = new System.Drawing.Point(3, 82);
			this.PrimaryRaysMapCoordinates.Name = "PrimaryRaysMapCoordinates";
			this.PrimaryRaysMapCoordinates.Size = new System.Drawing.Size(93, 60);
			this.PrimaryRaysMapCoordinates.TabIndex = 8;
			this.PrimaryRaysMapCoordinates.Text = "X: \r\nY: \r\nRays count:";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.RenderAllRaysMapButtona);
			this.tabPage3.Controls.Add(this.AllRaysMapPanel);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(801, 511);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "All Rays Map";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// RenderAllRaysMapButtona
			// 
			this.RenderAllRaysMapButtona.Location = new System.Drawing.Point(7, 6);
			this.RenderAllRaysMapButtona.Name = "RenderAllRaysMapButtona";
			this.RenderAllRaysMapButtona.Size = new System.Drawing.Size(97, 41);
			this.RenderAllRaysMapButtona.TabIndex = 11;
			this.RenderAllRaysMapButtona.Tag = "allRaysMap";
			this.RenderAllRaysMapButtona.Text = "Render";
			this.RenderAllRaysMapButtona.UseVisualStyleBackColor = true;
			this.RenderAllRaysMapButtona.Click += new System.EventHandler(this.RenderMapButton_Click);
			// 
			// AllRaysMapPanel
			// 
			this.AllRaysMapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.AllRaysMapPanel.AutoScroll = true;
			this.AllRaysMapPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.AllRaysMapPanel.Controls.Add(this.SaveAllRaysMapButton);
			this.AllRaysMapPanel.Controls.Add(this.AverageAllRaysCount);
			this.AllRaysMapPanel.Controls.Add(this.AllRaysMapPictureBox);
			this.AllRaysMapPanel.Controls.Add(this.TotalAllRaysCount);
			this.AllRaysMapPanel.Controls.Add(this.AllRaysMapCoordinates);
			this.AllRaysMapPanel.Location = new System.Drawing.Point(0, 0);
			this.AllRaysMapPanel.Name = "AllRaysMapPanel";
			this.AllRaysMapPanel.Size = new System.Drawing.Size(801, 511);
			this.AllRaysMapPanel.TabIndex = 10;
			this.AllRaysMapPanel.Tag = "AllRaysMap";
			// 
			// SaveAllRaysMapButton
			// 
			this.SaveAllRaysMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SaveAllRaysMapButton.Enabled = false;
			this.SaveAllRaysMapButton.Location = new System.Drawing.Point(7, 445);
			this.SaveAllRaysMapButton.Name = "SaveAllRaysMapButton";
			this.SaveAllRaysMapButton.Size = new System.Drawing.Size(97, 41);
			this.SaveAllRaysMapButton.TabIndex = 13;
			this.SaveAllRaysMapButton.Text = "Save Image";
			this.SaveAllRaysMapButton.UseVisualStyleBackColor = true;
			this.SaveAllRaysMapButton.Click += new System.EventHandler(this.SaveMapButton_Click);
			// 
			// AverageAllRaysCount
			// 
			this.AverageAllRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.AverageAllRaysCount.AutoSize = true;
			this.AverageAllRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AverageAllRaysCount.Location = new System.Drawing.Point(6, 292);
			this.AverageAllRaysCount.Name = "AverageAllRaysCount";
			this.AverageAllRaysCount.Size = new System.Drawing.Size(74, 60);
			this.AverageAllRaysCount.TabIndex = 11;
			this.AverageAllRaysCount.Text = "Average\r\nray count\r\nper pixel:\r\n";
			// 
			// AllRaysMapPictureBox
			// 
			this.AllRaysMapPictureBox.Location = new System.Drawing.Point(110, 6);
			this.AllRaysMapPictureBox.Name = "AllRaysMapPictureBox";
			this.AllRaysMapPictureBox.Size = new System.Drawing.Size(680, 480);
			this.AllRaysMapPictureBox.TabIndex = 7;
			this.AllRaysMapPictureBox.TabStop = false;
			this.AllRaysMapPictureBox.Tag = "allRaysMap";
			this.AllRaysMapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
			this.AllRaysMapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
			// 
			// TotalAllRaysCount
			// 
			this.TotalAllRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.TotalAllRaysCount.AutoSize = true;
			this.TotalAllRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TotalAllRaysCount.Location = new System.Drawing.Point(6, 179);
			this.TotalAllRaysCount.Name = "TotalAllRaysCount";
			this.TotalAllRaysCount.Size = new System.Drawing.Size(86, 40);
			this.TotalAllRaysCount.TabIndex = 10;
			this.TotalAllRaysCount.Text = "Total\r\nrays count:";
			// 
			// AllRaysMapCoordinates
			// 
			this.AllRaysMapCoordinates.AutoSize = true;
			this.AllRaysMapCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AllRaysMapCoordinates.Location = new System.Drawing.Point(3, 82);
			this.AllRaysMapCoordinates.Name = "AllRaysMapCoordinates";
			this.AllRaysMapCoordinates.Size = new System.Drawing.Size(93, 60);
			this.AllRaysMapCoordinates.TabIndex = 8;
			this.AllRaysMapCoordinates.Text = "X: \r\nY: \r\nRays count:";
			// 
			// NormalMap
			// 
			this.NormalMap.Controls.Add(this.RenderNormalMapRelativeButton);
			this.NormalMap.Controls.Add(this.NormalMapRelativePanel);
			this.NormalMap.Location = new System.Drawing.Point(4, 22);
			this.NormalMap.Name = "NormalMap";
			this.NormalMap.Size = new System.Drawing.Size(801, 511);
			this.NormalMap.TabIndex = 3;
			this.NormalMap.Text = "Normal Map Relative";
			this.NormalMap.UseVisualStyleBackColor = true;
			// 
			// RenderNormalMapRelativeButton
			// 
			this.RenderNormalMapRelativeButton.Location = new System.Drawing.Point(7, 6);
			this.RenderNormalMapRelativeButton.Name = "RenderNormalMapRelativeButton";
			this.RenderNormalMapRelativeButton.Size = new System.Drawing.Size(97, 41);
			this.RenderNormalMapRelativeButton.TabIndex = 14;
			this.RenderNormalMapRelativeButton.Tag = "normalMapRelative";
			this.RenderNormalMapRelativeButton.Text = "Render";
			this.RenderNormalMapRelativeButton.UseVisualStyleBackColor = true;
			this.RenderNormalMapRelativeButton.Click += new System.EventHandler(this.RenderMapButton_Click);
			// 
			// NormalMapRelativePanel
			// 
			this.NormalMapRelativePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.NormalMapRelativePanel.AutoScroll = true;
			this.NormalMapRelativePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.NormalMapRelativePanel.Controls.Add(this.SaveNormalMapRelativeButton);
			this.NormalMapRelativePanel.Controls.Add(this.NormalMapRelativePictureBox);
			this.NormalMapRelativePanel.Controls.Add(this.NormalMapRelativeCoordinates);
			this.NormalMapRelativePanel.Location = new System.Drawing.Point(0, 0);
			this.NormalMapRelativePanel.Name = "NormalMapRelativePanel";
			this.NormalMapRelativePanel.Size = new System.Drawing.Size(801, 511);
			this.NormalMapRelativePanel.TabIndex = 13;
			this.NormalMapRelativePanel.Tag = "NormalMapRelative";
			// 
			// SaveNormalMapRelativeButton
			// 
			this.SaveNormalMapRelativeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SaveNormalMapRelativeButton.Enabled = false;
			this.SaveNormalMapRelativeButton.Location = new System.Drawing.Point(7, 445);
			this.SaveNormalMapRelativeButton.Name = "SaveNormalMapRelativeButton";
			this.SaveNormalMapRelativeButton.Size = new System.Drawing.Size(97, 41);
			this.SaveNormalMapRelativeButton.TabIndex = 16;
			this.SaveNormalMapRelativeButton.Text = "Save Image";
			this.SaveNormalMapRelativeButton.UseVisualStyleBackColor = true;
			this.SaveNormalMapRelativeButton.Click += new System.EventHandler(this.SaveMapButton_Click);
			// 
			// NormalMapRelativePictureBox
			// 
			this.NormalMapRelativePictureBox.Location = new System.Drawing.Point(110, 6);
			this.NormalMapRelativePictureBox.Name = "NormalMapRelativePictureBox";
			this.NormalMapRelativePictureBox.Size = new System.Drawing.Size(680, 480);
			this.NormalMapRelativePictureBox.TabIndex = 7;
			this.NormalMapRelativePictureBox.TabStop = false;
			this.NormalMapRelativePictureBox.Tag = "normalMapRelative";
			this.NormalMapRelativePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
			this.NormalMapRelativePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
			// 
			// NormalMapRelativeCoordinates
			// 
			this.NormalMapRelativeCoordinates.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.NormalMapRelativeCoordinates.AutoSize = true;
			this.NormalMapRelativeCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NormalMapRelativeCoordinates.Location = new System.Drawing.Point(3, 82);
			this.NormalMapRelativeCoordinates.Name = "NormalMapRelativeCoordinates";
			this.NormalMapRelativeCoordinates.Size = new System.Drawing.Size(108, 80);
			this.NormalMapRelativeCoordinates.TabIndex = 8;
			this.NormalMapRelativeCoordinates.Text = "X: \r\nY: \r\nAngle of \r\nnormal vector:\r\n";
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.RenderNormalMapAbsoluteButton);
			this.tabPage5.Controls.Add(this.NormalMapAbsolutePanel);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Size = new System.Drawing.Size(801, 511);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Normal Map Absolute";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// RenderNormalMapAbsoluteButton
			// 
			this.RenderNormalMapAbsoluteButton.Location = new System.Drawing.Point(7, 6);
			this.RenderNormalMapAbsoluteButton.Name = "RenderNormalMapAbsoluteButton";
			this.RenderNormalMapAbsoluteButton.Size = new System.Drawing.Size(97, 41);
			this.RenderNormalMapAbsoluteButton.TabIndex = 16;
			this.RenderNormalMapAbsoluteButton.Tag = "normalMapAbsolute";
			this.RenderNormalMapAbsoluteButton.Text = "Render";
			this.RenderNormalMapAbsoluteButton.UseVisualStyleBackColor = true;
			this.RenderNormalMapAbsoluteButton.Click += new System.EventHandler(this.RenderMapButton_Click);
			// 
			// NormalMapAbsolutePanel
			// 
			this.NormalMapAbsolutePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.NormalMapAbsolutePanel.AutoScroll = true;
			this.NormalMapAbsolutePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.NormalMapAbsolutePanel.Controls.Add(this.SaveNormalMapAbsoluteButton);
			this.NormalMapAbsolutePanel.Controls.Add(this.NormalMapAbsolutePictureBox);
			this.NormalMapAbsolutePanel.Controls.Add(this.NormalMapAbsoluteCoordinates);
			this.NormalMapAbsolutePanel.Location = new System.Drawing.Point(0, 0);
			this.NormalMapAbsolutePanel.Name = "NormalMapAbsolutePanel";
			this.NormalMapAbsolutePanel.Size = new System.Drawing.Size(801, 511);
			this.NormalMapAbsolutePanel.TabIndex = 15;
			this.NormalMapAbsolutePanel.Tag = "NormalMapAbsolute";
			// 
			// SaveNormalMapAbsoluteButton
			// 
			this.SaveNormalMapAbsoluteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SaveNormalMapAbsoluteButton.Enabled = false;
			this.SaveNormalMapAbsoluteButton.Location = new System.Drawing.Point(7, 445);
			this.SaveNormalMapAbsoluteButton.Name = "SaveNormalMapAbsoluteButton";
			this.SaveNormalMapAbsoluteButton.Size = new System.Drawing.Size(97, 41);
			this.SaveNormalMapAbsoluteButton.TabIndex = 16;
			this.SaveNormalMapAbsoluteButton.Text = "Save Image";
			this.SaveNormalMapAbsoluteButton.UseVisualStyleBackColor = true;
			this.SaveNormalMapAbsoluteButton.Click += new System.EventHandler(this.SaveMapButton_Click);
			// 
			// NormalMapAbsolutePictureBox
			// 
			this.NormalMapAbsolutePictureBox.Location = new System.Drawing.Point(110, 6);
			this.NormalMapAbsolutePictureBox.Name = "NormalMapAbsolutePictureBox";
			this.NormalMapAbsolutePictureBox.Size = new System.Drawing.Size(680, 480);
			this.NormalMapAbsolutePictureBox.TabIndex = 7;
			this.NormalMapAbsolutePictureBox.TabStop = false;
			this.NormalMapAbsolutePictureBox.Tag = "normalMapAbsolute";
			this.NormalMapAbsolutePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
			this.NormalMapAbsolutePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
			// 
			// NormalMapAbsoluteCoordinates
			// 
			this.NormalMapAbsoluteCoordinates.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.NormalMapAbsoluteCoordinates.AutoSize = true;
			this.NormalMapAbsoluteCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NormalMapAbsoluteCoordinates.Location = new System.Drawing.Point(3, 82);
			this.NormalMapAbsoluteCoordinates.Name = "NormalMapAbsoluteCoordinates";
			this.NormalMapAbsoluteCoordinates.Size = new System.Drawing.Size(108, 80);
			this.NormalMapAbsoluteCoordinates.TabIndex = 8;
			this.NormalMapAbsoluteCoordinates.Text = "X: \r\nY: \r\nAngle of \r\nnormal vector:\r\n";
			// 
			// Form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(841, 561);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(350, 600);
			this.Name = "Form2";
			this.Text = "Advanced tools";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.DepthMapPanel.ResumeLayout(false);
			this.DepthMapPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.PrimaryRaysMapPanel.ResumeLayout(false);
			this.PrimaryRaysMapPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PrimaryRaysMapPictureBox)).EndInit();
			this.tabPage3.ResumeLayout(false);
			this.AllRaysMapPanel.ResumeLayout(false);
			this.AllRaysMapPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.AllRaysMapPictureBox)).EndInit();
			this.NormalMap.ResumeLayout(false);
			this.NormalMapRelativePanel.ResumeLayout(false);
			this.NormalMapRelativePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.NormalMapRelativePictureBox)).EndInit();
			this.tabPage5.ResumeLayout(false);
			this.NormalMapAbsolutePanel.ResumeLayout(false);
			this.NormalMapAbsolutePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.NormalMapAbsolutePictureBox)).EndInit();
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TabPage tabPage3;
    private System.Windows.Forms.TabPage NormalMap;
    private System.Windows.Forms.Panel DepthMapPanel;
    private System.Windows.Forms.Button SaveDepthMapButton;
    private System.Windows.Forms.Button RenderDepthMapButton;
    internal System.Windows.Forms.PictureBox DepthMapPictureBox;
    private System.Windows.Forms.Button SavePrimaryRaysMapButton;
    internal System.Windows.Forms.PictureBox PrimaryRaysMapPictureBox;
    private System.Windows.Forms.Button RenderPrimaryRaysMapButton;
    private System.Windows.Forms.Panel PrimaryRaysMapPanel;
    private System.Windows.Forms.Label DepthMap_Coordinates;
    private System.Windows.Forms.Label PrimaryRaysMapCoordinates;
    private System.Windows.Forms.Label AveragePrimaryRaysCount;
    private System.Windows.Forms.Label TotalPrimaryRaysCount;
    private System.Windows.Forms.Button RenderAllRaysMapButtona;
    private System.Windows.Forms.Panel AllRaysMapPanel;
    private System.Windows.Forms.Label AverageAllRaysCount;
    internal System.Windows.Forms.PictureBox AllRaysMapPictureBox;
    private System.Windows.Forms.Label TotalAllRaysCount;
    private System.Windows.Forms.Label AllRaysMapCoordinates;
    private System.Windows.Forms.TabPage tabPage5;
    private System.Windows.Forms.Button RenderNormalMapRelativeButton;
    private System.Windows.Forms.Panel NormalMapRelativePanel;
    internal System.Windows.Forms.PictureBox NormalMapRelativePictureBox;
    private System.Windows.Forms.Label NormalMapRelativeCoordinates;
    private System.Windows.Forms.Button SaveAllRaysMapButton;
    private System.Windows.Forms.Button SaveNormalMapRelativeButton;
    private System.Windows.Forms.Button RenderNormalMapAbsoluteButton;
    private System.Windows.Forms.Panel NormalMapAbsolutePanel;
    private System.Windows.Forms.Button SaveNormalMapAbsoluteButton;
    internal System.Windows.Forms.PictureBox NormalMapAbsolutePictureBox;
    private System.Windows.Forms.Label NormalMapAbsoluteCoordinates;
  }
}