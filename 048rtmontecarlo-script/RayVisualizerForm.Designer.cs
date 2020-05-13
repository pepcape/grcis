namespace Rendering
{
	partial class RayVisualizerForm
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
      this.glControl1 = new OpenTK.GLControl();
      this.checkAxes = new System.Windows.Forms.CheckBox();
      this.checkSpecular = new System.Windows.Forms.CheckBox();
      this.checkDiffuse = new System.Windows.Forms.CheckBox();
      this.checkAmbient = new System.Windows.Forms.CheckBox();
      this.checkShaders = new System.Windows.Forms.CheckBox();
      this.checkGlobalColor = new System.Windows.Forms.CheckBox();
      this.checkVsync = new System.Windows.Forms.CheckBox();
      this.checkSmooth = new System.Windows.Forms.CheckBox();
      this.labelFPS = new System.Windows.Forms.Label();
      this.NormalRaysCheckBox = new System.Windows.Forms.CheckBox();
      this.ShadowRaysCheckBox = new System.Windows.Forms.CheckBox();
      this.CameraCheckBox = new System.Windows.Forms.CheckBox();
      this.LightSourcesCheckBox = new System.Windows.Forms.CheckBox();
      this.AllignCameraButton = new System.Windows.Forms.Button();
      this.AllignCameraCheckBox = new System.Windows.Forms.CheckBox();
      this.BoundingBoxesCheckBox = new System.Windows.Forms.CheckBox();
      this.WireframeBoundingBoxesCheckBox = new System.Windows.Forms.CheckBox();
      this.PointCloudButton = new System.Windows.Forms.Button();
      this.PointCloudCheckBox = new System.Windows.Forms.CheckBox();
      this.SaveScreenshotButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // glControl1
      // 
      this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.glControl1.BackColor = System.Drawing.Color.Black;
      this.glControl1.Location = new System.Drawing.Point(12, 12);
      this.glControl1.Name = "glControl1";
      this.glControl1.Size = new System.Drawing.Size(960, 451);
      this.glControl1.TabIndex = 0;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
      this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
      this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
      this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
      // 
      // checkAxes
      // 
      this.checkAxes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAxes.AutoSize = true;
      this.checkAxes.Location = new System.Drawing.Point(194, 521);
      this.checkAxes.Name = "checkAxes";
      this.checkAxes.Size = new System.Drawing.Size(49, 17);
      this.checkAxes.TabIndex = 57;
      this.checkAxes.Text = "Axes";
      this.checkAxes.UseVisualStyleBackColor = true;
      // 
      // checkSpecular
      // 
      this.checkSpecular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSpecular.AutoSize = true;
      this.checkSpecular.Checked = true;
      this.checkSpecular.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkSpecular.Location = new System.Drawing.Point(108, 516);
      this.checkSpecular.Name = "checkSpecular";
      this.checkSpecular.Size = new System.Drawing.Size(68, 17);
      this.checkSpecular.TabIndex = 55;
      this.checkSpecular.Text = "Specular";
      this.checkSpecular.UseVisualStyleBackColor = true;
      // 
      // checkDiffuse
      // 
      this.checkDiffuse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDiffuse.AutoSize = true;
      this.checkDiffuse.Checked = true;
      this.checkDiffuse.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkDiffuse.Location = new System.Drawing.Point(108, 493);
      this.checkDiffuse.Name = "checkDiffuse";
      this.checkDiffuse.Size = new System.Drawing.Size(59, 17);
      this.checkDiffuse.TabIndex = 54;
      this.checkDiffuse.Text = "Diffuse";
      this.checkDiffuse.UseVisualStyleBackColor = true;
      // 
      // checkAmbient
      // 
      this.checkAmbient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAmbient.AutoSize = true;
      this.checkAmbient.Checked = true;
      this.checkAmbient.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkAmbient.Location = new System.Drawing.Point(108, 469);
      this.checkAmbient.Name = "checkAmbient";
      this.checkAmbient.Size = new System.Drawing.Size(64, 17);
      this.checkAmbient.TabIndex = 53;
      this.checkAmbient.Text = "Ambient";
      this.checkAmbient.UseVisualStyleBackColor = true;
      // 
      // checkShaders
      // 
      this.checkShaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkShaders.AutoSize = true;
      this.checkShaders.Checked = true;
      this.checkShaders.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkShaders.Location = new System.Drawing.Point(108, 539);
      this.checkShaders.Name = "checkShaders";
      this.checkShaders.Size = new System.Drawing.Size(60, 17);
      this.checkShaders.TabIndex = 52;
      this.checkShaders.Text = "Shader";
      this.checkShaders.UseVisualStyleBackColor = true;
      // 
      // checkGlobalColor
      // 
      this.checkGlobalColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkGlobalColor.AutoSize = true;
      this.checkGlobalColor.Location = new System.Drawing.Point(12, 516);
      this.checkGlobalColor.Name = "checkGlobalColor";
      this.checkGlobalColor.Size = new System.Drawing.Size(82, 17);
      this.checkGlobalColor.TabIndex = 51;
      this.checkGlobalColor.Text = "Global color";
      this.checkGlobalColor.UseVisualStyleBackColor = true;
      // 
      // checkVsync
      // 
      this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVsync.AutoSize = true;
      this.checkVsync.Checked = true;
      this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVsync.Location = new System.Drawing.Point(12, 493);
      this.checkVsync.Name = "checkVsync";
      this.checkVsync.Size = new System.Drawing.Size(60, 17);
      this.checkVsync.TabIndex = 49;
      this.checkVsync.Text = "V-Sync";
      this.checkVsync.UseVisualStyleBackColor = true;
      // 
      // checkSmooth
      // 
      this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSmooth.AutoSize = true;
      this.checkSmooth.Checked = true;
      this.checkSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkSmooth.Location = new System.Drawing.Point(12, 469);
      this.checkSmooth.Name = "checkSmooth";
      this.checkSmooth.Size = new System.Drawing.Size(62, 17);
      this.checkSmooth.TabIndex = 46;
      this.checkSmooth.Text = "Smooth";
      this.checkSmooth.UseVisualStyleBackColor = true;
      // 
      // labelFPS
      // 
      this.labelFPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelFPS.AutoSize = true;
      this.labelFPS.Location = new System.Drawing.Point(191, 544);
      this.labelFPS.Name = "labelFPS";
      this.labelFPS.Size = new System.Drawing.Size(33, 13);
      this.labelFPS.TabIndex = 58;
      this.labelFPS.Text = "FPS: ";
      // 
      // NormalRaysCheckBox
      // 
      this.NormalRaysCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.NormalRaysCheckBox.AutoSize = true;
      this.NormalRaysCheckBox.Checked = true;
      this.NormalRaysCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.NormalRaysCheckBox.Location = new System.Drawing.Point(510, 469);
      this.NormalRaysCheckBox.Name = "NormalRaysCheckBox";
      this.NormalRaysCheckBox.Size = new System.Drawing.Size(81, 17);
      this.NormalRaysCheckBox.TabIndex = 59;
      this.NormalRaysCheckBox.Text = "Normal rays";
      this.NormalRaysCheckBox.UseVisualStyleBackColor = true;
      // 
      // ShadowRaysCheckBox
      // 
      this.ShadowRaysCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.ShadowRaysCheckBox.AutoSize = true;
      this.ShadowRaysCheckBox.Checked = true;
      this.ShadowRaysCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ShadowRaysCheckBox.Location = new System.Drawing.Point(510, 493);
      this.ShadowRaysCheckBox.Name = "ShadowRaysCheckBox";
      this.ShadowRaysCheckBox.Size = new System.Drawing.Size(87, 17);
      this.ShadowRaysCheckBox.TabIndex = 60;
      this.ShadowRaysCheckBox.Text = "Shadow rays";
      this.ShadowRaysCheckBox.UseVisualStyleBackColor = true;
      // 
      // CameraCheckBox
      // 
      this.CameraCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.CameraCheckBox.AutoSize = true;
      this.CameraCheckBox.Checked = true;
      this.CameraCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CameraCheckBox.Location = new System.Drawing.Point(510, 516);
      this.CameraCheckBox.Name = "CameraCheckBox";
      this.CameraCheckBox.Size = new System.Drawing.Size(62, 17);
      this.CameraCheckBox.TabIndex = 61;
      this.CameraCheckBox.Text = "Camera";
      this.CameraCheckBox.UseVisualStyleBackColor = true;
      // 
      // LightSourcesCheckBox
      // 
      this.LightSourcesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.LightSourcesCheckBox.AutoSize = true;
      this.LightSourcesCheckBox.Checked = true;
      this.LightSourcesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.LightSourcesCheckBox.Location = new System.Drawing.Point(510, 541);
      this.LightSourcesCheckBox.Name = "LightSourcesCheckBox";
      this.LightSourcesCheckBox.Size = new System.Drawing.Size(89, 17);
      this.LightSourcesCheckBox.TabIndex = 62;
      this.LightSourcesCheckBox.Text = "Light sources";
      this.LightSourcesCheckBox.UseVisualStyleBackColor = true;
      // 
      // AllignCameraButton
      // 
      this.AllignCameraButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.AllignCameraButton.Location = new System.Drawing.Point(674, 469);
      this.AllignCameraButton.Name = "AllignCameraButton";
      this.AllignCameraButton.Size = new System.Drawing.Size(159, 23);
      this.AllignCameraButton.TabIndex = 63;
      this.AllignCameraButton.Text = "Allign camera to primary ray";
      this.AllignCameraButton.UseVisualStyleBackColor = true;
      this.AllignCameraButton.Click += new System.EventHandler(this.AllignCamera);
      // 
      // AllignCameraCheckBox
      // 
      this.AllignCameraCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.AllignCameraCheckBox.AutoSize = true;
      this.AllignCameraCheckBox.Location = new System.Drawing.Point(840, 474);
      this.AllignCameraCheckBox.Name = "AllignCameraCheckBox";
      this.AllignCameraCheckBox.Size = new System.Drawing.Size(90, 17);
      this.AllignCameraCheckBox.TabIndex = 64;
      this.AllignCameraCheckBox.Text = "Keep alligned";
      this.AllignCameraCheckBox.UseVisualStyleBackColor = true;
      // 
      // BoundingBoxesCheckBox
      // 
      this.BoundingBoxesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.BoundingBoxesCheckBox.AutoSize = true;
      this.BoundingBoxesCheckBox.Checked = true;
      this.BoundingBoxesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.BoundingBoxesCheckBox.Location = new System.Drawing.Point(617, 518);
      this.BoundingBoxesCheckBox.Name = "BoundingBoxesCheckBox";
      this.BoundingBoxesCheckBox.Size = new System.Drawing.Size(102, 17);
      this.BoundingBoxesCheckBox.TabIndex = 65;
      this.BoundingBoxesCheckBox.Text = "Bounding boxes";
      this.BoundingBoxesCheckBox.UseVisualStyleBackColor = true;
      this.BoundingBoxesCheckBox.CheckedChanged += new System.EventHandler(this.BoundingBoxesCheckBox_CheckedChanged);
      // 
      // WireframeBoundingBoxesCheckBox
      // 
      this.WireframeBoundingBoxesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.WireframeBoundingBoxesCheckBox.AutoSize = true;
      this.WireframeBoundingBoxesCheckBox.Checked = true;
      this.WireframeBoundingBoxesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.WireframeBoundingBoxesCheckBox.Location = new System.Drawing.Point(617, 541);
      this.WireframeBoundingBoxesCheckBox.Name = "WireframeBoundingBoxesCheckBox";
      this.WireframeBoundingBoxesCheckBox.Size = new System.Drawing.Size(164, 17);
      this.WireframeBoundingBoxesCheckBox.TabIndex = 66;
      this.WireframeBoundingBoxesCheckBox.Text = "Bounding boxes as wireframe";
      this.WireframeBoundingBoxesCheckBox.UseVisualStyleBackColor = true;
      // 
      // PointCloudButton
      // 
      this.PointCloudButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.PointCloudButton.Enabled = false;
      this.PointCloudButton.Location = new System.Drawing.Point(194, 469);
      this.PointCloudButton.Name = "PointCloudButton";
      this.PointCloudButton.Size = new System.Drawing.Size(102, 23);
      this.PointCloudButton.TabIndex = 67;
      this.PointCloudButton.Text = "Load point cloud";
      this.PointCloudButton.UseVisualStyleBackColor = true;
      this.PointCloudButton.Click += new System.EventHandler(this.PointCloudButton_Click);
      // 
      // PointCloudCheckBox
      // 
      this.PointCloudCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.PointCloudCheckBox.AutoSize = true;
      this.PointCloudCheckBox.Enabled = false;
      this.PointCloudCheckBox.Location = new System.Drawing.Point(194, 498);
      this.PointCloudCheckBox.Name = "PointCloudCheckBox";
      this.PointCloudCheckBox.Size = new System.Drawing.Size(79, 17);
      this.PointCloudCheckBox.TabIndex = 68;
      this.PointCloudCheckBox.Text = "Point cloud";
      this.PointCloudCheckBox.UseVisualStyleBackColor = true;
      // 
      // SaveScreenshotButton
      // 
      this.SaveScreenshotButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.SaveScreenshotButton.Location = new System.Drawing.Point(875, 535);
      this.SaveScreenshotButton.Name = "SaveScreenshotButton";
      this.SaveScreenshotButton.Size = new System.Drawing.Size(97, 23);
      this.SaveScreenshotButton.TabIndex = 69;
      this.SaveScreenshotButton.Text = "Save Screenshot";
      this.SaveScreenshotButton.UseVisualStyleBackColor = true;
      this.SaveScreenshotButton.Click += new System.EventHandler(this.SaveScreenshotButton_Click);
      // 
      // RayVisualizerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(984, 570);
      this.Controls.Add(this.SaveScreenshotButton);
      this.Controls.Add(this.PointCloudCheckBox);
      this.Controls.Add(this.PointCloudButton);
      this.Controls.Add(this.WireframeBoundingBoxesCheckBox);
      this.Controls.Add(this.BoundingBoxesCheckBox);
      this.Controls.Add(this.AllignCameraCheckBox);
      this.Controls.Add(this.AllignCameraButton);
      this.Controls.Add(this.LightSourcesCheckBox);
      this.Controls.Add(this.CameraCheckBox);
      this.Controls.Add(this.ShadowRaysCheckBox);
      this.Controls.Add(this.NormalRaysCheckBox);
      this.Controls.Add(this.labelFPS);
      this.Controls.Add(this.checkAxes);
      this.Controls.Add(this.checkSpecular);
      this.Controls.Add(this.checkDiffuse);
      this.Controls.Add(this.checkAmbient);
      this.Controls.Add(this.checkShaders);
      this.Controls.Add(this.checkGlobalColor);
      this.Controls.Add(this.checkVsync);
      this.Controls.Add(this.checkSmooth);
      this.Controls.Add(this.glControl1);
      this.MinimumSize = new System.Drawing.Size(1000, 500);
      this.Name = "RayVisualizerForm";
      this.Text = "Ray Visualizer";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RayVisualizerForm_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RayVisualizerForm_FormClosed);
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion

		private OpenTK.GLControl glControl1;
		private System.Windows.Forms.CheckBox checkAxes;
		private System.Windows.Forms.CheckBox checkSpecular;
		private System.Windows.Forms.CheckBox checkDiffuse;
		private System.Windows.Forms.CheckBox checkAmbient;
		private System.Windows.Forms.CheckBox checkShaders;
		private System.Windows.Forms.CheckBox checkGlobalColor;
		private System.Windows.Forms.CheckBox checkVsync;
		private System.Windows.Forms.CheckBox checkSmooth;
		private System.Windows.Forms.Label labelFPS;
		private System.Windows.Forms.CheckBox NormalRaysCheckBox;
		private System.Windows.Forms.CheckBox ShadowRaysCheckBox;
		private System.Windows.Forms.CheckBox CameraCheckBox;
		private System.Windows.Forms.CheckBox LightSourcesCheckBox;
		private System.Windows.Forms.Button AllignCameraButton;
		private System.Windows.Forms.CheckBox AllignCameraCheckBox;
		private System.Windows.Forms.CheckBox BoundingBoxesCheckBox;
		private System.Windows.Forms.CheckBox WireframeBoundingBoxesCheckBox;
    private System.Windows.Forms.CheckBox PointCloudCheckBox;
    public System.Windows.Forms.Button PointCloudButton;
    private System.Windows.Forms.Button SaveScreenshotButton;
  }
}