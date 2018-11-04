namespace Rendering
{
	partial class AdvancedToolsForm
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
      this.MapsTabControl = new System.Windows.Forms.TabControl();
      this.DepthMapTab = new System.Windows.Forms.TabPage();
      this.DepthMapLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.DepthMapPictureBox = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.SaveDepthMapButton = new System.Windows.Forms.Button();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.RenderDepthMapButton = new System.Windows.Forms.Button();
      this.DepthMap_Coordinates = new System.Windows.Forms.Label();
      this.PrimaryRaysMapTab = new System.Windows.Forms.TabPage();
      this.PrimaryRaysMapLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.PrimaryRaysMapPictureBox = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.SavePrimaryRaysMapButton = new System.Windows.Forms.Button();
      this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
      this.RenderPrimaryRaysMapButton = new System.Windows.Forms.Button();
      this.PrimaryRaysMapCoordinates = new System.Windows.Forms.Label();
      this.TotalPrimaryRaysCount = new System.Windows.Forms.Label();
      this.AveragePrimaryRaysCount = new System.Windows.Forms.Label();
      this.AllRaysMapTab = new System.Windows.Forms.TabPage();
      this.AllRaysMapLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.AllRaysMapPictureBox = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.SaveAllRaysMapButton = new System.Windows.Forms.Button();
      this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
      this.RenderAllRaysMapButton = new System.Windows.Forms.Button();
      this.AllRaysMapCoordinates = new System.Windows.Forms.Label();
      this.TotalAllRaysCount = new System.Windows.Forms.Label();
      this.AverageAllRaysCount = new System.Windows.Forms.Label();
      this.NormalMapRelativeTab = new System.Windows.Forms.TabPage();
      this.NormalMapRelativeLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.NormalMapRelativePictureBox = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.SaveNormalMapRelativeButton = new System.Windows.Forms.Button();
      this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
      this.RenderNormalMapRelativeButton = new System.Windows.Forms.Button();
      this.NormalMapRelativeCoordinates = new System.Windows.Forms.Label();
      this.NormalMapAbsoluteTab = new System.Windows.Forms.TabPage();
      this.NormalMapAbsoluteLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.NormalMapAbsolutePictureBox = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
      this.SaveNormalMapAbsoluteButton = new System.Windows.Forms.Button();
      this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
      this.RenderNormalMapAbsoluteButton = new System.Windows.Forms.Button();
      this.NormalMapAbsoluteCoordinates = new System.Windows.Forms.Label();
      this.MapsTabControl.SuspendLayout();
      this.DepthMapTab.SuspendLayout();
      this.DepthMapLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.PrimaryRaysMapTab.SuspendLayout();
      this.PrimaryRaysMapLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PrimaryRaysMapPictureBox)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      this.flowLayoutPanel2.SuspendLayout();
      this.AllRaysMapTab.SuspendLayout();
      this.AllRaysMapLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.AllRaysMapPictureBox)).BeginInit();
      this.tableLayoutPanel4.SuspendLayout();
      this.flowLayoutPanel3.SuspendLayout();
      this.NormalMapRelativeTab.SuspendLayout();
      this.NormalMapRelativeLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.NormalMapRelativePictureBox)).BeginInit();
      this.tableLayoutPanel5.SuspendLayout();
      this.flowLayoutPanel4.SuspendLayout();
      this.NormalMapAbsoluteTab.SuspendLayout();
      this.NormalMapAbsoluteLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.NormalMapAbsolutePictureBox)).BeginInit();
      this.tableLayoutPanel6.SuspendLayout();
      this.flowLayoutPanel5.SuspendLayout();
      this.SuspendLayout();
      // 
      // MapsTabControl
      // 
      this.MapsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.MapsTabControl.Controls.Add(this.DepthMapTab);
      this.MapsTabControl.Controls.Add(this.PrimaryRaysMapTab);
      this.MapsTabControl.Controls.Add(this.AllRaysMapTab);
      this.MapsTabControl.Controls.Add(this.NormalMapRelativeTab);
      this.MapsTabControl.Controls.Add(this.NormalMapAbsoluteTab);
      this.MapsTabControl.Location = new System.Drawing.Point(12, 12);
      this.MapsTabControl.Margin = new System.Windows.Forms.Padding(0);
      this.MapsTabControl.Name = "MapsTabControl";
      this.MapsTabControl.SelectedIndex = 0;
      this.MapsTabControl.Size = new System.Drawing.Size(861, 559);
      this.MapsTabControl.TabIndex = 0;
      this.MapsTabControl.Tag = "";
      // 
      // DepthMapTab
      // 
      this.DepthMapTab.Controls.Add(this.DepthMapLayoutPanel);
      this.DepthMapTab.Location = new System.Drawing.Point(4, 22);
      this.DepthMapTab.Margin = new System.Windows.Forms.Padding(0);
      this.DepthMapTab.Name = "DepthMapTab";
      this.DepthMapTab.Size = new System.Drawing.Size(853, 533);
      this.DepthMapTab.TabIndex = 5;
      this.DepthMapTab.Tag = "DepthMap";
      this.DepthMapTab.Text = "Depth Map";
      this.DepthMapTab.UseVisualStyleBackColor = true;
      // 
      // DepthMapLayoutPanel
      // 
      this.DepthMapLayoutPanel.ColumnCount = 2;
      this.DepthMapLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
      this.DepthMapLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.DepthMapLayoutPanel.Controls.Add(this.DepthMapPictureBox, 1, 0);
      this.DepthMapLayoutPanel.Controls.Add(this.tableLayoutPanel2, 0, 0);
      this.DepthMapLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.DepthMapLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.DepthMapLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.DepthMapLayoutPanel.Name = "DepthMapLayoutPanel";
      this.DepthMapLayoutPanel.RowCount = 1;
      this.DepthMapLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.DepthMapLayoutPanel.Size = new System.Drawing.Size(853, 533);
      this.DepthMapLayoutPanel.TabIndex = 0;
      this.DepthMapLayoutPanel.Tag = "";
      // 
      // DepthMapPictureBox
      // 
      this.DepthMapPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.DepthMapPictureBox.Location = new System.Drawing.Point(175, 0);
      this.DepthMapPictureBox.Margin = new System.Windows.Forms.Padding(0);
      this.DepthMapPictureBox.Name = "DepthMapPictureBox";
      this.DepthMapPictureBox.Size = new System.Drawing.Size(678, 533);
      this.DepthMapPictureBox.TabIndex = 4;
      this.DepthMapPictureBox.TabStop = false;
      this.DepthMapPictureBox.Tag = "";
      this.DepthMapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DepthMapPictureBox_MouseDownAndMouseMove);
      this.DepthMapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DepthMapPictureBox_MouseDownAndMouseMove);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.SaveDepthMapButton, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(175, 533);
      this.tableLayoutPanel2.TabIndex = 5;
      // 
      // SaveDepthMapButton
      // 
      this.SaveDepthMapButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.SaveDepthMapButton.Enabled = false;
      this.SaveDepthMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SaveDepthMapButton.Location = new System.Drawing.Point(3, 476);
      this.SaveDepthMapButton.Name = "SaveDepthMapButton";
      this.SaveDepthMapButton.Size = new System.Drawing.Size(169, 54);
      this.SaveDepthMapButton.TabIndex = 6;
      this.SaveDepthMapButton.Text = "Save Image";
      this.SaveDepthMapButton.UseVisualStyleBackColor = true;
      this.SaveDepthMapButton.Click += new System.EventHandler(this.SaveMapButton_Click);
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Controls.Add(this.RenderDepthMapButton);
      this.flowLayoutPanel1.Controls.Add(this.DepthMap_Coordinates);
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(175, 473);
      this.flowLayoutPanel1.TabIndex = 7;
      // 
      // RenderDepthMapButton
      // 
      this.RenderDepthMapButton.Dock = System.Windows.Forms.DockStyle.Top;
      this.RenderDepthMapButton.Enabled = false;
      this.RenderDepthMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RenderDepthMapButton.Location = new System.Drawing.Point(3, 3);
      this.RenderDepthMapButton.Name = "RenderDepthMapButton";
      this.RenderDepthMapButton.Size = new System.Drawing.Size(169, 54);
      this.RenderDepthMapButton.TabIndex = 7;
      this.RenderDepthMapButton.Text = "Render";
      this.RenderDepthMapButton.UseVisualStyleBackColor = true;
      this.RenderDepthMapButton.Click += new System.EventHandler(this.RenderMapButton_Click);
      // 
      // DepthMap_Coordinates
      // 
      this.DepthMap_Coordinates.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.DepthMap_Coordinates.AutoSize = true;
      this.DepthMap_Coordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DepthMap_Coordinates.Location = new System.Drawing.Point(0, 75);
      this.DepthMap_Coordinates.Margin = new System.Windows.Forms.Padding(0, 15, 0, 0);
      this.DepthMap_Coordinates.Name = "DepthMap_Coordinates";
      this.DepthMap_Coordinates.Size = new System.Drawing.Size(61, 60);
      this.DepthMap_Coordinates.TabIndex = 8;
      this.DepthMap_Coordinates.Text = "X: \r\nY: \r\nDepth: \r\n";
      // 
      // PrimaryRaysMapTab
      // 
      this.PrimaryRaysMapTab.Controls.Add(this.PrimaryRaysMapLayoutPanel);
      this.PrimaryRaysMapTab.Location = new System.Drawing.Point(4, 22);
      this.PrimaryRaysMapTab.Margin = new System.Windows.Forms.Padding(0);
      this.PrimaryRaysMapTab.Name = "PrimaryRaysMapTab";
      this.PrimaryRaysMapTab.Size = new System.Drawing.Size(853, 533);
      this.PrimaryRaysMapTab.TabIndex = 6;
      this.PrimaryRaysMapTab.Tag = "PrimaryRaysMap";
      this.PrimaryRaysMapTab.Text = "Primary Rays Map";
      this.PrimaryRaysMapTab.UseVisualStyleBackColor = true;
      // 
      // PrimaryRaysMapLayoutPanel
      // 
      this.PrimaryRaysMapLayoutPanel.ColumnCount = 2;
      this.PrimaryRaysMapLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
      this.PrimaryRaysMapLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.PrimaryRaysMapLayoutPanel.Controls.Add(this.PrimaryRaysMapPictureBox, 1, 0);
      this.PrimaryRaysMapLayoutPanel.Controls.Add(this.tableLayoutPanel3, 0, 0);
      this.PrimaryRaysMapLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.PrimaryRaysMapLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.PrimaryRaysMapLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.PrimaryRaysMapLayoutPanel.Name = "PrimaryRaysMapLayoutPanel";
      this.PrimaryRaysMapLayoutPanel.RowCount = 1;
      this.PrimaryRaysMapLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.PrimaryRaysMapLayoutPanel.Size = new System.Drawing.Size(853, 533);
      this.PrimaryRaysMapLayoutPanel.TabIndex = 1;
      this.PrimaryRaysMapLayoutPanel.Tag = "";
      // 
      // PrimaryRaysMapPictureBox
      // 
      this.PrimaryRaysMapPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.PrimaryRaysMapPictureBox.Location = new System.Drawing.Point(175, 0);
      this.PrimaryRaysMapPictureBox.Margin = new System.Windows.Forms.Padding(0);
      this.PrimaryRaysMapPictureBox.Name = "PrimaryRaysMapPictureBox";
      this.PrimaryRaysMapPictureBox.Size = new System.Drawing.Size(678, 533);
      this.PrimaryRaysMapPictureBox.TabIndex = 4;
      this.PrimaryRaysMapPictureBox.TabStop = false;
      this.PrimaryRaysMapPictureBox.Tag = "";
      this.PrimaryRaysMapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
      this.PrimaryRaysMapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.SavePrimaryRaysMapButton, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel2, 0, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(175, 533);
      this.tableLayoutPanel3.TabIndex = 5;
      // 
      // SavePrimaryRaysMapButton
      // 
      this.SavePrimaryRaysMapButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.SavePrimaryRaysMapButton.Enabled = false;
      this.SavePrimaryRaysMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SavePrimaryRaysMapButton.Location = new System.Drawing.Point(3, 476);
      this.SavePrimaryRaysMapButton.Name = "SavePrimaryRaysMapButton";
      this.SavePrimaryRaysMapButton.Size = new System.Drawing.Size(169, 54);
      this.SavePrimaryRaysMapButton.TabIndex = 6;
      this.SavePrimaryRaysMapButton.Text = "Save Image";
      this.SavePrimaryRaysMapButton.UseVisualStyleBackColor = true;
      this.SavePrimaryRaysMapButton.Click += new System.EventHandler(this.SaveMapButton_Click);
      // 
      // flowLayoutPanel2
      // 
      this.flowLayoutPanel2.Controls.Add(this.RenderPrimaryRaysMapButton);
      this.flowLayoutPanel2.Controls.Add(this.PrimaryRaysMapCoordinates);
      this.flowLayoutPanel2.Controls.Add(this.TotalPrimaryRaysCount);
      this.flowLayoutPanel2.Controls.Add(this.AveragePrimaryRaysCount);
      this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel2.Name = "flowLayoutPanel2";
      this.flowLayoutPanel2.Size = new System.Drawing.Size(175, 473);
      this.flowLayoutPanel2.TabIndex = 7;
      // 
      // RenderPrimaryRaysMapButton
      // 
      this.RenderPrimaryRaysMapButton.Dock = System.Windows.Forms.DockStyle.Top;
      this.RenderPrimaryRaysMapButton.Enabled = false;
      this.RenderPrimaryRaysMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RenderPrimaryRaysMapButton.Location = new System.Drawing.Point(3, 3);
      this.RenderPrimaryRaysMapButton.Name = "RenderPrimaryRaysMapButton";
      this.RenderPrimaryRaysMapButton.Size = new System.Drawing.Size(169, 54);
      this.RenderPrimaryRaysMapButton.TabIndex = 7;
      this.RenderPrimaryRaysMapButton.Text = "Render";
      this.RenderPrimaryRaysMapButton.UseVisualStyleBackColor = true;
      this.RenderPrimaryRaysMapButton.Click += new System.EventHandler(this.RenderMapButton_Click);
      // 
      // PrimaryRaysMapCoordinates
      // 
      this.PrimaryRaysMapCoordinates.AutoSize = true;
      this.PrimaryRaysMapCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PrimaryRaysMapCoordinates.Location = new System.Drawing.Point(0, 75);
      this.PrimaryRaysMapCoordinates.Margin = new System.Windows.Forms.Padding(0, 15, 0, 0);
      this.PrimaryRaysMapCoordinates.Name = "PrimaryRaysMapCoordinates";
      this.PrimaryRaysMapCoordinates.Size = new System.Drawing.Size(93, 60);
      this.PrimaryRaysMapCoordinates.TabIndex = 12;
      this.PrimaryRaysMapCoordinates.Text = "X: \r\nY: \r\nRays count:";
      // 
      // TotalPrimaryRaysCount
      // 
      this.TotalPrimaryRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.TotalPrimaryRaysCount.AutoSize = true;
      this.flowLayoutPanel2.SetFlowBreak(this.TotalPrimaryRaysCount, true);
      this.TotalPrimaryRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TotalPrimaryRaysCount.Location = new System.Drawing.Point(0, 175);
      this.TotalPrimaryRaysCount.Margin = new System.Windows.Forms.Padding(0, 40, 0, 0);
      this.TotalPrimaryRaysCount.Name = "TotalPrimaryRaysCount";
      this.TotalPrimaryRaysCount.Size = new System.Drawing.Size(136, 40);
      this.TotalPrimaryRaysCount.TabIndex = 13;
      this.TotalPrimaryRaysCount.Text = "Total primary rays count:\r\n";
      // 
      // AveragePrimaryRaysCount
      // 
      this.AveragePrimaryRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.AveragePrimaryRaysCount.AutoSize = true;
      this.flowLayoutPanel2.SetFlowBreak(this.AveragePrimaryRaysCount, true);
      this.AveragePrimaryRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.AveragePrimaryRaysCount.Location = new System.Drawing.Point(0, 255);
      this.AveragePrimaryRaysCount.Margin = new System.Windows.Forms.Padding(0, 40, 0, 0);
      this.AveragePrimaryRaysCount.Name = "AveragePrimaryRaysCount";
      this.AveragePrimaryRaysCount.Size = new System.Drawing.Size(160, 40);
      this.AveragePrimaryRaysCount.TabIndex = 14;
      this.AveragePrimaryRaysCount.Text = "Average primary rays count per pixel:\r\n";
      // 
      // AllRaysMapTab
      // 
      this.AllRaysMapTab.Controls.Add(this.AllRaysMapLayoutPanel);
      this.AllRaysMapTab.Location = new System.Drawing.Point(4, 22);
      this.AllRaysMapTab.Margin = new System.Windows.Forms.Padding(0);
      this.AllRaysMapTab.Name = "AllRaysMapTab";
      this.AllRaysMapTab.Size = new System.Drawing.Size(853, 533);
      this.AllRaysMapTab.TabIndex = 7;
      this.AllRaysMapTab.Tag = "AllRaysMap";
      this.AllRaysMapTab.Text = "All Rays Map";
      this.AllRaysMapTab.UseVisualStyleBackColor = true;
      // 
      // AllRaysMapLayoutPanel
      // 
      this.AllRaysMapLayoutPanel.ColumnCount = 2;
      this.AllRaysMapLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
      this.AllRaysMapLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.AllRaysMapLayoutPanel.Controls.Add(this.AllRaysMapPictureBox, 1, 0);
      this.AllRaysMapLayoutPanel.Controls.Add(this.tableLayoutPanel4, 0, 0);
      this.AllRaysMapLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.AllRaysMapLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.AllRaysMapLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.AllRaysMapLayoutPanel.Name = "AllRaysMapLayoutPanel";
      this.AllRaysMapLayoutPanel.RowCount = 1;
      this.AllRaysMapLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.AllRaysMapLayoutPanel.Size = new System.Drawing.Size(853, 533);
      this.AllRaysMapLayoutPanel.TabIndex = 2;
      this.AllRaysMapLayoutPanel.Tag = "";
      // 
      // AllRaysMapPictureBox
      // 
      this.AllRaysMapPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.AllRaysMapPictureBox.Location = new System.Drawing.Point(175, 0);
      this.AllRaysMapPictureBox.Margin = new System.Windows.Forms.Padding(0);
      this.AllRaysMapPictureBox.Name = "AllRaysMapPictureBox";
      this.AllRaysMapPictureBox.Size = new System.Drawing.Size(678, 533);
      this.AllRaysMapPictureBox.TabIndex = 4;
      this.AllRaysMapPictureBox.TabStop = false;
      this.AllRaysMapPictureBox.Tag = "";
      this.AllRaysMapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
      this.AllRaysMapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RaysMapPictureBox_MouseDownAndMouseMove);
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 1;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Controls.Add(this.SaveAllRaysMapButton, 0, 1);
      this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel3, 0, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(175, 533);
      this.tableLayoutPanel4.TabIndex = 5;
      // 
      // SaveAllRaysMapButton
      // 
      this.SaveAllRaysMapButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.SaveAllRaysMapButton.Enabled = false;
      this.SaveAllRaysMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SaveAllRaysMapButton.Location = new System.Drawing.Point(3, 476);
      this.SaveAllRaysMapButton.Name = "SaveAllRaysMapButton";
      this.SaveAllRaysMapButton.Size = new System.Drawing.Size(169, 54);
      this.SaveAllRaysMapButton.TabIndex = 6;
      this.SaveAllRaysMapButton.Text = "Save Image";
      this.SaveAllRaysMapButton.UseVisualStyleBackColor = true;
      this.SaveAllRaysMapButton.Click += new System.EventHandler(this.SaveMapButton_Click);
      // 
      // flowLayoutPanel3
      // 
      this.flowLayoutPanel3.Controls.Add(this.RenderAllRaysMapButton);
      this.flowLayoutPanel3.Controls.Add(this.AllRaysMapCoordinates);
      this.flowLayoutPanel3.Controls.Add(this.TotalAllRaysCount);
      this.flowLayoutPanel3.Controls.Add(this.AverageAllRaysCount);
      this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel3.Name = "flowLayoutPanel3";
      this.flowLayoutPanel3.Size = new System.Drawing.Size(175, 473);
      this.flowLayoutPanel3.TabIndex = 7;
      // 
      // RenderAllRaysMapButton
      // 
      this.RenderAllRaysMapButton.Dock = System.Windows.Forms.DockStyle.Top;
      this.RenderAllRaysMapButton.Enabled = false;
      this.RenderAllRaysMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RenderAllRaysMapButton.Location = new System.Drawing.Point(3, 3);
      this.RenderAllRaysMapButton.Name = "RenderAllRaysMapButton";
      this.RenderAllRaysMapButton.Size = new System.Drawing.Size(169, 54);
      this.RenderAllRaysMapButton.TabIndex = 7;
      this.RenderAllRaysMapButton.Text = "Render";
      this.RenderAllRaysMapButton.UseVisualStyleBackColor = true;
      this.RenderAllRaysMapButton.Click += new System.EventHandler(this.RenderMapButton_Click);
      // 
      // AllRaysMapCoordinates
      // 
      this.AllRaysMapCoordinates.AutoSize = true;
      this.AllRaysMapCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.AllRaysMapCoordinates.Location = new System.Drawing.Point(0, 75);
      this.AllRaysMapCoordinates.Margin = new System.Windows.Forms.Padding(0, 15, 0, 0);
      this.AllRaysMapCoordinates.Name = "AllRaysMapCoordinates";
      this.AllRaysMapCoordinates.Size = new System.Drawing.Size(93, 60);
      this.AllRaysMapCoordinates.TabIndex = 12;
      this.AllRaysMapCoordinates.Text = "X: \r\nY: \r\nRays count:";
      // 
      // TotalAllRaysCount
      // 
      this.TotalAllRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.TotalAllRaysCount.AutoSize = true;
      this.flowLayoutPanel3.SetFlowBreak(this.TotalAllRaysCount, true);
      this.TotalAllRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TotalAllRaysCount.Location = new System.Drawing.Point(0, 175);
      this.TotalAllRaysCount.Margin = new System.Windows.Forms.Padding(0, 40, 0, 0);
      this.TotalAllRaysCount.Name = "TotalAllRaysCount";
      this.TotalAllRaysCount.Size = new System.Drawing.Size(125, 20);
      this.TotalAllRaysCount.TabIndex = 13;
      this.TotalAllRaysCount.Text = "Total rays count:\r\n";
      // 
      // AverageAllRaysCount
      // 
      this.AverageAllRaysCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.AverageAllRaysCount.AutoSize = true;
      this.flowLayoutPanel3.SetFlowBreak(this.AverageAllRaysCount, true);
      this.AverageAllRaysCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.AverageAllRaysCount.Location = new System.Drawing.Point(0, 235);
      this.AverageAllRaysCount.Margin = new System.Windows.Forms.Padding(0, 40, 0, 0);
      this.AverageAllRaysCount.Name = "AverageAllRaysCount";
      this.AverageAllRaysCount.Size = new System.Drawing.Size(172, 40);
      this.AverageAllRaysCount.TabIndex = 14;
      this.AverageAllRaysCount.Text = "Average rays count per pixel:\r\n";
      // 
      // NormalMapRelativeTab
      // 
      this.NormalMapRelativeTab.Controls.Add(this.NormalMapRelativeLayoutPanel);
      this.NormalMapRelativeTab.Location = new System.Drawing.Point(4, 22);
      this.NormalMapRelativeTab.Margin = new System.Windows.Forms.Padding(0);
      this.NormalMapRelativeTab.Name = "NormalMapRelativeTab";
      this.NormalMapRelativeTab.Size = new System.Drawing.Size(853, 533);
      this.NormalMapRelativeTab.TabIndex = 8;
      this.NormalMapRelativeTab.Tag = "NormalMapRelative";
      this.NormalMapRelativeTab.Text = "Normal Map Relative";
      this.NormalMapRelativeTab.UseVisualStyleBackColor = true;
      // 
      // NormalMapRelativeLayoutPanel
      // 
      this.NormalMapRelativeLayoutPanel.ColumnCount = 2;
      this.NormalMapRelativeLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
      this.NormalMapRelativeLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.NormalMapRelativeLayoutPanel.Controls.Add(this.NormalMapRelativePictureBox, 1, 0);
      this.NormalMapRelativeLayoutPanel.Controls.Add(this.tableLayoutPanel5, 0, 0);
      this.NormalMapRelativeLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.NormalMapRelativeLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.NormalMapRelativeLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.NormalMapRelativeLayoutPanel.Name = "NormalMapRelativeLayoutPanel";
      this.NormalMapRelativeLayoutPanel.RowCount = 1;
      this.NormalMapRelativeLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.NormalMapRelativeLayoutPanel.Size = new System.Drawing.Size(853, 533);
      this.NormalMapRelativeLayoutPanel.TabIndex = 3;
      this.NormalMapRelativeLayoutPanel.Tag = "";
      // 
      // NormalMapRelativePictureBox
      // 
      this.NormalMapRelativePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.NormalMapRelativePictureBox.Location = new System.Drawing.Point(175, 0);
      this.NormalMapRelativePictureBox.Margin = new System.Windows.Forms.Padding(0);
      this.NormalMapRelativePictureBox.Name = "NormalMapRelativePictureBox";
      this.NormalMapRelativePictureBox.Size = new System.Drawing.Size(678, 533);
      this.NormalMapRelativePictureBox.TabIndex = 4;
      this.NormalMapRelativePictureBox.TabStop = false;
      this.NormalMapRelativePictureBox.Tag = "";
      this.NormalMapRelativePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
      this.NormalMapRelativePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 1;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Controls.Add(this.SaveNormalMapRelativeButton, 0, 1);
      this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel4, 0, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 2;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(175, 533);
      this.tableLayoutPanel5.TabIndex = 5;
      // 
      // SaveNormalMapRelativeButton
      // 
      this.SaveNormalMapRelativeButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.SaveNormalMapRelativeButton.Enabled = false;
      this.SaveNormalMapRelativeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SaveNormalMapRelativeButton.Location = new System.Drawing.Point(3, 476);
      this.SaveNormalMapRelativeButton.Name = "SaveNormalMapRelativeButton";
      this.SaveNormalMapRelativeButton.Size = new System.Drawing.Size(169, 54);
      this.SaveNormalMapRelativeButton.TabIndex = 6;
      this.SaveNormalMapRelativeButton.Text = "Save Image";
      this.SaveNormalMapRelativeButton.UseVisualStyleBackColor = true;
      this.SaveNormalMapRelativeButton.Click += new System.EventHandler(this.SaveMapButton_Click);
      // 
      // flowLayoutPanel4
      // 
      this.flowLayoutPanel4.Controls.Add(this.RenderNormalMapRelativeButton);
      this.flowLayoutPanel4.Controls.Add(this.NormalMapRelativeCoordinates);
      this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel4.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel4.Name = "flowLayoutPanel4";
      this.flowLayoutPanel4.Size = new System.Drawing.Size(175, 473);
      this.flowLayoutPanel4.TabIndex = 7;
      // 
      // RenderNormalMapRelativeButton
      // 
      this.RenderNormalMapRelativeButton.Dock = System.Windows.Forms.DockStyle.Top;
      this.RenderNormalMapRelativeButton.Enabled = false;
      this.RenderNormalMapRelativeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RenderNormalMapRelativeButton.Location = new System.Drawing.Point(3, 3);
      this.RenderNormalMapRelativeButton.Name = "RenderNormalMapRelativeButton";
      this.RenderNormalMapRelativeButton.Size = new System.Drawing.Size(169, 54);
      this.RenderNormalMapRelativeButton.TabIndex = 7;
      this.RenderNormalMapRelativeButton.Text = "Render";
      this.RenderNormalMapRelativeButton.UseVisualStyleBackColor = true;
      this.RenderNormalMapRelativeButton.Click += new System.EventHandler(this.RenderMapButton_Click);
      // 
      // NormalMapRelativeCoordinates
      // 
      this.NormalMapRelativeCoordinates.AutoSize = true;
      this.NormalMapRelativeCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.NormalMapRelativeCoordinates.Location = new System.Drawing.Point(0, 75);
      this.NormalMapRelativeCoordinates.Margin = new System.Windows.Forms.Padding(0, 15, 0, 0);
      this.NormalMapRelativeCoordinates.Name = "NormalMapRelativeCoordinates";
      this.NormalMapRelativeCoordinates.Size = new System.Drawing.Size(171, 60);
      this.NormalMapRelativeCoordinates.TabIndex = 12;
      this.NormalMapRelativeCoordinates.Text = "X: \r\nY: \r\nAngle of normal vector:\r\n";
      // 
      // NormalMapAbsoluteTab
      // 
      this.NormalMapAbsoluteTab.Controls.Add(this.NormalMapAbsoluteLayoutPanel);
      this.NormalMapAbsoluteTab.Location = new System.Drawing.Point(4, 22);
      this.NormalMapAbsoluteTab.Margin = new System.Windows.Forms.Padding(0);
      this.NormalMapAbsoluteTab.Name = "NormalMapAbsoluteTab";
      this.NormalMapAbsoluteTab.Size = new System.Drawing.Size(853, 533);
      this.NormalMapAbsoluteTab.TabIndex = 9;
      this.NormalMapAbsoluteTab.Tag = "NormalMapAbsolute";
      this.NormalMapAbsoluteTab.Text = "Normal Map Absolute";
      this.NormalMapAbsoluteTab.UseVisualStyleBackColor = true;
      // 
      // NormalMapAbsoluteLayoutPanel
      // 
      this.NormalMapAbsoluteLayoutPanel.ColumnCount = 2;
      this.NormalMapAbsoluteLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
      this.NormalMapAbsoluteLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.NormalMapAbsoluteLayoutPanel.Controls.Add(this.NormalMapAbsolutePictureBox, 1, 0);
      this.NormalMapAbsoluteLayoutPanel.Controls.Add(this.tableLayoutPanel6, 0, 0);
      this.NormalMapAbsoluteLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.NormalMapAbsoluteLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.NormalMapAbsoluteLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.NormalMapAbsoluteLayoutPanel.Name = "NormalMapAbsoluteLayoutPanel";
      this.NormalMapAbsoluteLayoutPanel.RowCount = 1;
      this.NormalMapAbsoluteLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.NormalMapAbsoluteLayoutPanel.Size = new System.Drawing.Size(853, 533);
      this.NormalMapAbsoluteLayoutPanel.TabIndex = 4;
      this.NormalMapAbsoluteLayoutPanel.Tag = "";
      // 
      // NormalMapAbsolutePictureBox
      // 
      this.NormalMapAbsolutePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.NormalMapAbsolutePictureBox.Location = new System.Drawing.Point(175, 0);
      this.NormalMapAbsolutePictureBox.Margin = new System.Windows.Forms.Padding(0);
      this.NormalMapAbsolutePictureBox.Name = "NormalMapAbsolutePictureBox";
      this.NormalMapAbsolutePictureBox.Size = new System.Drawing.Size(678, 533);
      this.NormalMapAbsolutePictureBox.TabIndex = 4;
      this.NormalMapAbsolutePictureBox.TabStop = false;
      this.NormalMapAbsolutePictureBox.Tag = "";
      this.NormalMapAbsolutePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
      this.NormalMapAbsolutePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NormalMapPictureBox_MouseDownAndMouseMove);
      // 
      // tableLayoutPanel6
      // 
      this.tableLayoutPanel6.ColumnCount = 1;
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel6.Controls.Add(this.SaveNormalMapAbsoluteButton, 0, 1);
      this.tableLayoutPanel6.Controls.Add(this.flowLayoutPanel5, 0, 0);
      this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel6.Name = "tableLayoutPanel6";
      this.tableLayoutPanel6.RowCount = 2;
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel6.Size = new System.Drawing.Size(175, 533);
      this.tableLayoutPanel6.TabIndex = 5;
      // 
      // SaveNormalMapAbsoluteButton
      // 
      this.SaveNormalMapAbsoluteButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.SaveNormalMapAbsoluteButton.Enabled = false;
      this.SaveNormalMapAbsoluteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SaveNormalMapAbsoluteButton.Location = new System.Drawing.Point(3, 476);
      this.SaveNormalMapAbsoluteButton.Name = "SaveNormalMapAbsoluteButton";
      this.SaveNormalMapAbsoluteButton.Size = new System.Drawing.Size(169, 54);
      this.SaveNormalMapAbsoluteButton.TabIndex = 6;
      this.SaveNormalMapAbsoluteButton.Text = "Save Image";
      this.SaveNormalMapAbsoluteButton.UseVisualStyleBackColor = true;
      this.SaveNormalMapAbsoluteButton.Click += new System.EventHandler(this.SaveMapButton_Click);
      // 
      // flowLayoutPanel5
      // 
      this.flowLayoutPanel5.Controls.Add(this.RenderNormalMapAbsoluteButton);
      this.flowLayoutPanel5.Controls.Add(this.NormalMapAbsoluteCoordinates);
      this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel5.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel5.Name = "flowLayoutPanel5";
      this.flowLayoutPanel5.Size = new System.Drawing.Size(175, 473);
      this.flowLayoutPanel5.TabIndex = 7;
      // 
      // RenderNormalMapAbsoluteButton
      // 
      this.RenderNormalMapAbsoluteButton.Dock = System.Windows.Forms.DockStyle.Top;
      this.RenderNormalMapAbsoluteButton.Enabled = false;
      this.RenderNormalMapAbsoluteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RenderNormalMapAbsoluteButton.Location = new System.Drawing.Point(3, 3);
      this.RenderNormalMapAbsoluteButton.Name = "RenderNormalMapAbsoluteButton";
      this.RenderNormalMapAbsoluteButton.Size = new System.Drawing.Size(169, 54);
      this.RenderNormalMapAbsoluteButton.TabIndex = 7;
      this.RenderNormalMapAbsoluteButton.Text = "Render";
      this.RenderNormalMapAbsoluteButton.UseVisualStyleBackColor = true;
      this.RenderNormalMapAbsoluteButton.Click += new System.EventHandler(this.RenderMapButton_Click);
      // 
      // NormalMapAbsoluteCoordinates
      // 
      this.NormalMapAbsoluteCoordinates.AutoSize = true;
      this.NormalMapAbsoluteCoordinates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.NormalMapAbsoluteCoordinates.Location = new System.Drawing.Point(0, 75);
      this.NormalMapAbsoluteCoordinates.Margin = new System.Windows.Forms.Padding(0, 15, 0, 0);
      this.NormalMapAbsoluteCoordinates.Name = "NormalMapAbsoluteCoordinates";
      this.NormalMapAbsoluteCoordinates.Size = new System.Drawing.Size(171, 60);
      this.NormalMapAbsoluteCoordinates.TabIndex = 12;
      this.NormalMapAbsoluteCoordinates.Text = "X: \r\nY: \r\nAngle of normal vector:\r\n";
      // 
      // AdvancedToolsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(893, 583);
      this.Controls.Add(this.MapsTabControl);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.MinimumSize = new System.Drawing.Size(600, 470);
      this.Name = "AdvancedToolsForm";
      this.Text = "Advanced tools";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
      this.MapsTabControl.ResumeLayout(false);
      this.DepthMapTab.ResumeLayout(false);
      this.DepthMapLayoutPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.DepthMapPictureBox)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.PrimaryRaysMapTab.ResumeLayout(false);
      this.PrimaryRaysMapLayoutPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.PrimaryRaysMapPictureBox)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.flowLayoutPanel2.ResumeLayout(false);
      this.flowLayoutPanel2.PerformLayout();
      this.AllRaysMapTab.ResumeLayout(false);
      this.AllRaysMapLayoutPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.AllRaysMapPictureBox)).EndInit();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.flowLayoutPanel3.ResumeLayout(false);
      this.flowLayoutPanel3.PerformLayout();
      this.NormalMapRelativeTab.ResumeLayout(false);
      this.NormalMapRelativeLayoutPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.NormalMapRelativePictureBox)).EndInit();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.flowLayoutPanel4.ResumeLayout(false);
      this.flowLayoutPanel4.PerformLayout();
      this.NormalMapAbsoluteTab.ResumeLayout(false);
      this.NormalMapAbsoluteLayoutPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.NormalMapAbsolutePictureBox)).EndInit();
      this.tableLayoutPanel6.ResumeLayout(false);
      this.flowLayoutPanel5.ResumeLayout(false);
      this.flowLayoutPanel5.PerformLayout();
      this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl MapsTabControl;
    private System.Windows.Forms.TabPage DepthMapTab;
    private System.Windows.Forms.TableLayoutPanel DepthMapLayoutPanel;
    internal System.Windows.Forms.PictureBox DepthMapPictureBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button SaveDepthMapButton;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Button RenderDepthMapButton;
    private System.Windows.Forms.Label DepthMap_Coordinates;
    private System.Windows.Forms.TabPage PrimaryRaysMapTab;
    private System.Windows.Forms.TableLayoutPanel PrimaryRaysMapLayoutPanel;
    internal System.Windows.Forms.PictureBox PrimaryRaysMapPictureBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button SavePrimaryRaysMapButton;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    private System.Windows.Forms.Button RenderPrimaryRaysMapButton;
    private System.Windows.Forms.Label PrimaryRaysMapCoordinates;
    private System.Windows.Forms.Label TotalPrimaryRaysCount;
    private System.Windows.Forms.Label AveragePrimaryRaysCount;
    private System.Windows.Forms.TabPage AllRaysMapTab;
    private System.Windows.Forms.TableLayoutPanel AllRaysMapLayoutPanel;
    internal System.Windows.Forms.PictureBox AllRaysMapPictureBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Button SaveAllRaysMapButton;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
    private System.Windows.Forms.Button RenderAllRaysMapButton;
    private System.Windows.Forms.Label AllRaysMapCoordinates;
    private System.Windows.Forms.Label TotalAllRaysCount;
    private System.Windows.Forms.Label AverageAllRaysCount;
    private System.Windows.Forms.TabPage NormalMapRelativeTab;
    private System.Windows.Forms.TableLayoutPanel NormalMapRelativeLayoutPanel;
    internal System.Windows.Forms.PictureBox NormalMapRelativePictureBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Button SaveNormalMapRelativeButton;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
    private System.Windows.Forms.Button RenderNormalMapRelativeButton;
    private System.Windows.Forms.Label NormalMapRelativeCoordinates;
    private System.Windows.Forms.TabPage NormalMapAbsoluteTab;
    private System.Windows.Forms.TableLayoutPanel NormalMapAbsoluteLayoutPanel;
    internal System.Windows.Forms.PictureBox NormalMapAbsolutePictureBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    private System.Windows.Forms.Button SaveNormalMapAbsoluteButton;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
    private System.Windows.Forms.Button RenderNormalMapAbsoluteButton;
    private System.Windows.Forms.Label NormalMapAbsoluteCoordinates;
  }
}