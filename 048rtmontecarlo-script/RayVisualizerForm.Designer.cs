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
			this.glControl1 = new OpenTK.GLControl ();
			this.checkAxes = new System.Windows.Forms.CheckBox ();
			this.checkPhong = new System.Windows.Forms.CheckBox ();
			this.checkSpecular = new System.Windows.Forms.CheckBox ();
			this.checkDiffuse = new System.Windows.Forms.CheckBox ();
			this.checkAmbient = new System.Windows.Forms.CheckBox ();
			this.checkShaders = new System.Windows.Forms.CheckBox ();
			this.checkGlobalColor = new System.Windows.Forms.CheckBox ();
			this.checkTexture = new System.Windows.Forms.CheckBox ();
			this.checkVsync = new System.Windows.Forms.CheckBox ();
			this.checkTwosided = new System.Windows.Forms.CheckBox ();
			this.checkWireframe = new System.Windows.Forms.CheckBox ();
			this.checkSmooth = new System.Windows.Forms.CheckBox ();
			this.labelFPS = new System.Windows.Forms.Label ();
			this.NormalRaysCheckBox = new System.Windows.Forms.CheckBox ();
			this.ShadowRaysCheckBox = new System.Windows.Forms.CheckBox ();
			this.CameraCheckBox = new System.Windows.Forms.CheckBox ();
			this.LightSourcesCheckBox = new System.Windows.Forms.CheckBox ();
			this.AllignCameraButton = new System.Windows.Forms.Button ();
			this.AllignCameraCheckBox = new System.Windows.Forms.CheckBox ();
			this.BoundingBoxesCheckBox = new System.Windows.Forms.CheckBox ();
			this.WireframeBoundingBoxesCheckBox = new System.Windows.Forms.CheckBox ();
			this.SuspendLayout ();
			// 
			// glControl1
			// 
			this.glControl1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
				  | System.Windows.Forms.AnchorStyles.Left )
				  | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.glControl1.BackColor = System.Drawing.Color.Black;
			this.glControl1.Location = new System.Drawing.Point ( 12, 12 );
			this.glControl1.Name = "glControl1";
			this.glControl1.Size = new System.Drawing.Size ( 960, 412 );
			this.glControl1.TabIndex = 0;
			this.glControl1.VSync = false;
			this.glControl1.Load += new System.EventHandler ( this.glControl1_Load );
			this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler ( this.glControl1_Paint );
			this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler ( this.glControl1_KeyDown );
			this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler ( this.glControl1_KeyUp );
			this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler ( this.glControl1_MouseDown );
			this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler ( this.glControl1_MouseMove );
			this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler ( this.glControl1_MouseUp );
			this.glControl1.Resize += new System.EventHandler ( this.glControl1_Resize );
			// 
			// checkAxes
			// 
			this.checkAxes.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkAxes.AutoSize = true;
			this.checkAxes.Location = new System.Drawing.Point ( 202, 502 );
			this.checkAxes.Name = "checkAxes";
			this.checkAxes.Size = new System.Drawing.Size ( 49, 17 );
			this.checkAxes.TabIndex = 57;
			this.checkAxes.Text = "Axes";
			this.checkAxes.UseVisualStyleBackColor = true;
			// 
			// checkPhong
			// 
			this.checkPhong.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkPhong.AutoSize = true;
			this.checkPhong.Checked = true;
			this.checkPhong.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkPhong.Location = new System.Drawing.Point ( 108, 502 );
			this.checkPhong.Name = "checkPhong";
			this.checkPhong.Size = new System.Drawing.Size ( 57, 17 );
			this.checkPhong.TabIndex = 56;
			this.checkPhong.Text = "Phong";
			this.checkPhong.UseVisualStyleBackColor = true;
			// 
			// checkSpecular
			// 
			this.checkSpecular.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkSpecular.AutoSize = true;
			this.checkSpecular.Checked = true;
			this.checkSpecular.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkSpecular.Location = new System.Drawing.Point ( 202, 477 );
			this.checkSpecular.Name = "checkSpecular";
			this.checkSpecular.Size = new System.Drawing.Size ( 68, 17 );
			this.checkSpecular.TabIndex = 55;
			this.checkSpecular.Text = "Specular";
			this.checkSpecular.UseVisualStyleBackColor = true;
			// 
			// checkDiffuse
			// 
			this.checkDiffuse.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkDiffuse.AutoSize = true;
			this.checkDiffuse.Checked = true;
			this.checkDiffuse.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkDiffuse.Location = new System.Drawing.Point ( 202, 454 );
			this.checkDiffuse.Name = "checkDiffuse";
			this.checkDiffuse.Size = new System.Drawing.Size ( 59, 17 );
			this.checkDiffuse.TabIndex = 54;
			this.checkDiffuse.Text = "Diffuse";
			this.checkDiffuse.UseVisualStyleBackColor = true;
			// 
			// checkAmbient
			// 
			this.checkAmbient.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkAmbient.AutoSize = true;
			this.checkAmbient.Checked = true;
			this.checkAmbient.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAmbient.Location = new System.Drawing.Point ( 202, 430 );
			this.checkAmbient.Name = "checkAmbient";
			this.checkAmbient.Size = new System.Drawing.Size ( 64, 17 );
			this.checkAmbient.TabIndex = 53;
			this.checkAmbient.Text = "Ambient";
			this.checkAmbient.UseVisualStyleBackColor = true;
			// 
			// checkShaders
			// 
			this.checkShaders.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkShaders.AutoSize = true;
			this.checkShaders.Location = new System.Drawing.Point ( 12, 502 );
			this.checkShaders.Name = "checkShaders";
			this.checkShaders.Size = new System.Drawing.Size ( 53, 17 );
			this.checkShaders.TabIndex = 52;
			this.checkShaders.Text = "GLSL";
			this.checkShaders.UseVisualStyleBackColor = true;
			// 
			// checkGlobalColor
			// 
			this.checkGlobalColor.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkGlobalColor.AutoSize = true;
			this.checkGlobalColor.Location = new System.Drawing.Point ( 108, 453 );
			this.checkGlobalColor.Name = "checkGlobalColor";
			this.checkGlobalColor.Size = new System.Drawing.Size ( 63, 17 );
			this.checkGlobalColor.TabIndex = 51;
			this.checkGlobalColor.Text = "GlobalC";
			this.checkGlobalColor.UseVisualStyleBackColor = true;
			// 
			// checkTexture
			// 
			this.checkTexture.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkTexture.AutoSize = true;
			this.checkTexture.Location = new System.Drawing.Point ( 108, 430 );
			this.checkTexture.Name = "checkTexture";
			this.checkTexture.Size = new System.Drawing.Size ( 62, 17 );
			this.checkTexture.TabIndex = 50;
			this.checkTexture.Text = "Texture";
			this.checkTexture.UseVisualStyleBackColor = true;
			// 
			// checkVsync
			// 
			this.checkVsync.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkVsync.AutoSize = true;
			this.checkVsync.Checked = true;
			this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkVsync.Location = new System.Drawing.Point ( 108, 476 );
			this.checkVsync.Name = "checkVsync";
			this.checkVsync.Size = new System.Drawing.Size ( 60, 17 );
			this.checkVsync.TabIndex = 49;
			this.checkVsync.Text = "V-Sync";
			this.checkVsync.UseVisualStyleBackColor = true;
			// 
			// checkTwosided
			// 
			this.checkTwosided.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkTwosided.AutoSize = true;
			this.checkTwosided.Checked = true;
			this.checkTwosided.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkTwosided.Location = new System.Drawing.Point ( 12, 477 );
			this.checkTwosided.Name = "checkTwosided";
			this.checkTwosided.Size = new System.Drawing.Size ( 60, 17 );
			this.checkTwosided.TabIndex = 48;
			this.checkTwosided.Text = "2-sided";
			this.checkTwosided.UseVisualStyleBackColor = true;
			// 
			// checkWireframe
			// 
			this.checkWireframe.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkWireframe.AutoSize = true;
			this.checkWireframe.Location = new System.Drawing.Point ( 12, 454 );
			this.checkWireframe.Name = "checkWireframe";
			this.checkWireframe.Size = new System.Drawing.Size ( 74, 17 );
			this.checkWireframe.TabIndex = 47;
			this.checkWireframe.Text = "Wireframe";
			this.checkWireframe.UseVisualStyleBackColor = true;
			// 
			// checkSmooth
			// 
			this.checkSmooth.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.checkSmooth.AutoSize = true;
			this.checkSmooth.Checked = true;
			this.checkSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkSmooth.Location = new System.Drawing.Point ( 12, 430 );
			this.checkSmooth.Name = "checkSmooth";
			this.checkSmooth.Size = new System.Drawing.Size ( 62, 17 );
			this.checkSmooth.TabIndex = 46;
			this.checkSmooth.Text = "Smooth";
			this.checkSmooth.UseVisualStyleBackColor = true;
			// 
			// labelFPS
			// 
			this.labelFPS.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.labelFPS.AutoSize = true;
			this.labelFPS.Location = new System.Drawing.Point ( 292, 506 );
			this.labelFPS.Name = "labelFPS";
			this.labelFPS.Size = new System.Drawing.Size ( 33, 13 );
			this.labelFPS.TabIndex = 58;
			this.labelFPS.Text = "FPS: ";
			// 
			// NormalRaysCheckBox
			// 
			this.NormalRaysCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.NormalRaysCheckBox.AutoSize = true;
			this.NormalRaysCheckBox.Checked = true;
			this.NormalRaysCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.NormalRaysCheckBox.Location = new System.Drawing.Point ( 510, 430 );
			this.NormalRaysCheckBox.Name = "NormalRaysCheckBox";
			this.NormalRaysCheckBox.Size = new System.Drawing.Size ( 81, 17 );
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
			this.ShadowRaysCheckBox.Location = new System.Drawing.Point ( 510, 454 );
			this.ShadowRaysCheckBox.Name = "ShadowRaysCheckBox";
			this.ShadowRaysCheckBox.Size = new System.Drawing.Size ( 87, 17 );
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
			this.CameraCheckBox.Location = new System.Drawing.Point ( 510, 477 );
			this.CameraCheckBox.Name = "CameraCheckBox";
			this.CameraCheckBox.Size = new System.Drawing.Size ( 62, 17 );
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
			this.LightSourcesCheckBox.Location = new System.Drawing.Point ( 510, 502 );
			this.LightSourcesCheckBox.Name = "LightSourcesCheckBox";
			this.LightSourcesCheckBox.Size = new System.Drawing.Size ( 89, 17 );
			this.LightSourcesCheckBox.TabIndex = 62;
			this.LightSourcesCheckBox.Text = "Light sources";
			this.LightSourcesCheckBox.UseVisualStyleBackColor = true;
			// 
			// AllignCameraButton
			// 
			this.AllignCameraButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.AllignCameraButton.Location = new System.Drawing.Point ( 674, 430 );
			this.AllignCameraButton.Name = "AllignCameraButton";
			this.AllignCameraButton.Size = new System.Drawing.Size ( 159, 23 );
			this.AllignCameraButton.TabIndex = 63;
			this.AllignCameraButton.Text = "Allign camera to primary ray";
			this.AllignCameraButton.UseVisualStyleBackColor = true;
			this.AllignCameraButton.Click += new System.EventHandler ( this.AllignCamera );
			// 
			// AllignCameraCheckBox
			// 
			this.AllignCameraCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.AllignCameraCheckBox.AutoSize = true;
			this.AllignCameraCheckBox.Location = new System.Drawing.Point ( 840, 435 );
			this.AllignCameraCheckBox.Name = "AllignCameraCheckBox";
			this.AllignCameraCheckBox.Size = new System.Drawing.Size ( 90, 17 );
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
			this.BoundingBoxesCheckBox.Location = new System.Drawing.Point ( 617, 479 );
			this.BoundingBoxesCheckBox.Name = "BoundingBoxesCheckBox";
			this.BoundingBoxesCheckBox.Size = new System.Drawing.Size ( 102, 17 );
			this.BoundingBoxesCheckBox.TabIndex = 65;
			this.BoundingBoxesCheckBox.Text = "Bounding boxes";
			this.BoundingBoxesCheckBox.UseVisualStyleBackColor = true;
			// 
			// WireframeBoundingBoxesCheckBox
			// 
			this.WireframeBoundingBoxesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.WireframeBoundingBoxesCheckBox.AutoSize = true;
			this.WireframeBoundingBoxesCheckBox.Checked = true;
			this.WireframeBoundingBoxesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.WireframeBoundingBoxesCheckBox.Location = new System.Drawing.Point ( 617, 502 );
			this.WireframeBoundingBoxesCheckBox.Name = "WireframeBoundingBoxesCheckBox";
			this.WireframeBoundingBoxesCheckBox.Size = new System.Drawing.Size ( 164, 17 );
			this.WireframeBoundingBoxesCheckBox.TabIndex = 66;
			this.WireframeBoundingBoxesCheckBox.Text = "Bounding boxes as wireframe";
			this.WireframeBoundingBoxesCheckBox.UseVisualStyleBackColor = true;
			// 
			// RayVisualizerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size ( 984, 531 );
			this.Controls.Add ( this.WireframeBoundingBoxesCheckBox );
			this.Controls.Add ( this.BoundingBoxesCheckBox );
			this.Controls.Add ( this.AllignCameraCheckBox );
			this.Controls.Add ( this.AllignCameraButton );
			this.Controls.Add ( this.LightSourcesCheckBox );
			this.Controls.Add ( this.CameraCheckBox );
			this.Controls.Add ( this.ShadowRaysCheckBox );
			this.Controls.Add ( this.NormalRaysCheckBox );
			this.Controls.Add ( this.labelFPS );
			this.Controls.Add ( this.checkAxes );
			this.Controls.Add ( this.checkPhong );
			this.Controls.Add ( this.checkSpecular );
			this.Controls.Add ( this.checkDiffuse );
			this.Controls.Add ( this.checkAmbient );
			this.Controls.Add ( this.checkShaders );
			this.Controls.Add ( this.checkGlobalColor );
			this.Controls.Add ( this.checkTexture );
			this.Controls.Add ( this.checkVsync );
			this.Controls.Add ( this.checkTwosided );
			this.Controls.Add ( this.checkWireframe );
			this.Controls.Add ( this.checkSmooth );
			this.Controls.Add ( this.glControl1 );
			this.MinimumSize = new System.Drawing.Size ( 1000, 500 );
			this.Name = "RayVisualizerForm";
			this.Text = "Ray Visualizer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler ( this.Form3_FormClosing );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler ( this.Form3_FormClosed );
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler ( this.glControl1_MouseWheel );
			this.ResumeLayout ( false );
			this.PerformLayout ();

		}

		#endregion

		private OpenTK.GLControl glControl1;
		private System.Windows.Forms.CheckBox checkAxes;
		private System.Windows.Forms.CheckBox checkPhong;
		private System.Windows.Forms.CheckBox checkSpecular;
		private System.Windows.Forms.CheckBox checkDiffuse;
		private System.Windows.Forms.CheckBox checkAmbient;
		private System.Windows.Forms.CheckBox checkShaders;
		private System.Windows.Forms.CheckBox checkGlobalColor;
		private System.Windows.Forms.CheckBox checkTexture;
		private System.Windows.Forms.CheckBox checkVsync;
		private System.Windows.Forms.CheckBox checkTwosided;
		private System.Windows.Forms.CheckBox checkWireframe;
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
	}
}