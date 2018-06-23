namespace Rendering
{
	partial class Form3
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
			this.glControl1 = new OpenTK.GLControl();
			this.checkAxes = new System.Windows.Forms.CheckBox();
			this.checkPhong = new System.Windows.Forms.CheckBox();
			this.checkSpecular = new System.Windows.Forms.CheckBox();
			this.checkDiffuse = new System.Windows.Forms.CheckBox();
			this.checkAmbient = new System.Windows.Forms.CheckBox();
			this.checkShaders = new System.Windows.Forms.CheckBox();
			this.checkGlobalColor = new System.Windows.Forms.CheckBox();
			this.checkTexture = new System.Windows.Forms.CheckBox();
			this.checkVsync = new System.Windows.Forms.CheckBox();
			this.checkTwosided = new System.Windows.Forms.CheckBox();
			this.checkWireframe = new System.Windows.Forms.CheckBox();
			this.checkSmooth = new System.Windows.Forms.CheckBox();
			this.labelFPS = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// glControl1
			// 
			this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.glControl1.BackColor = System.Drawing.Color.Black;
			this.glControl1.Location = new System.Drawing.Point(12, 12);
			this.glControl1.Name = "glControl1";
			this.glControl1.Size = new System.Drawing.Size(999, 409);
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
			this.checkAxes.Location = new System.Drawing.Point(202, 499);
			this.checkAxes.Name = "checkAxes";
			this.checkAxes.Size = new System.Drawing.Size(49, 17);
			this.checkAxes.TabIndex = 57;
			this.checkAxes.Text = "Axes";
			this.checkAxes.UseVisualStyleBackColor = true;
			// 
			// checkPhong
			// 
			this.checkPhong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkPhong.AutoSize = true;
			this.checkPhong.Checked = true;
			this.checkPhong.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkPhong.Location = new System.Drawing.Point(108, 499);
			this.checkPhong.Name = "checkPhong";
			this.checkPhong.Size = new System.Drawing.Size(57, 17);
			this.checkPhong.TabIndex = 56;
			this.checkPhong.Text = "Phong";
			this.checkPhong.UseVisualStyleBackColor = true;
			// 
			// checkSpecular
			// 
			this.checkSpecular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkSpecular.AutoSize = true;
			this.checkSpecular.Checked = true;
			this.checkSpecular.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkSpecular.Location = new System.Drawing.Point(202, 474);
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
			this.checkDiffuse.Location = new System.Drawing.Point(202, 451);
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
			this.checkAmbient.Location = new System.Drawing.Point(202, 427);
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
			this.checkShaders.Location = new System.Drawing.Point(12, 499);
			this.checkShaders.Name = "checkShaders";
			this.checkShaders.Size = new System.Drawing.Size(53, 17);
			this.checkShaders.TabIndex = 52;
			this.checkShaders.Text = "GLSL";
			this.checkShaders.UseVisualStyleBackColor = true;
			// 
			// checkGlobalColor
			// 
			this.checkGlobalColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkGlobalColor.AutoSize = true;
			this.checkGlobalColor.Location = new System.Drawing.Point(108, 450);
			this.checkGlobalColor.Name = "checkGlobalColor";
			this.checkGlobalColor.Size = new System.Drawing.Size(63, 17);
			this.checkGlobalColor.TabIndex = 51;
			this.checkGlobalColor.Text = "GlobalC";
			this.checkGlobalColor.UseVisualStyleBackColor = true;
			// 
			// checkTexture
			// 
			this.checkTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkTexture.AutoSize = true;
			this.checkTexture.Location = new System.Drawing.Point(108, 427);
			this.checkTexture.Name = "checkTexture";
			this.checkTexture.Size = new System.Drawing.Size(44, 17);
			this.checkTexture.TabIndex = 50;
			this.checkTexture.Text = "Tex";
			this.checkTexture.UseVisualStyleBackColor = true;
			// 
			// checkVsync
			// 
			this.checkVsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkVsync.AutoSize = true;
			this.checkVsync.Checked = true;
			this.checkVsync.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkVsync.Location = new System.Drawing.Point(108, 473);
			this.checkVsync.Name = "checkVsync";
			this.checkVsync.Size = new System.Drawing.Size(57, 17);
			this.checkVsync.TabIndex = 49;
			this.checkVsync.Text = "VSync";
			this.checkVsync.UseVisualStyleBackColor = true;
			// 
			// checkTwosided
			// 
			this.checkTwosided.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkTwosided.AutoSize = true;
			this.checkTwosided.Checked = true;
			this.checkTwosided.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkTwosided.Location = new System.Drawing.Point(12, 474);
			this.checkTwosided.Name = "checkTwosided";
			this.checkTwosided.Size = new System.Drawing.Size(57, 17);
			this.checkTwosided.TabIndex = 48;
			this.checkTwosided.Text = "2sided";
			this.checkTwosided.UseVisualStyleBackColor = true;
			// 
			// checkWireframe
			// 
			this.checkWireframe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkWireframe.AutoSize = true;
			this.checkWireframe.Location = new System.Drawing.Point(12, 451);
			this.checkWireframe.Name = "checkWireframe";
			this.checkWireframe.Size = new System.Drawing.Size(48, 17);
			this.checkWireframe.TabIndex = 47;
			this.checkWireframe.Text = "Wire";
			this.checkWireframe.UseVisualStyleBackColor = true;
			// 
			// checkSmooth
			// 
			this.checkSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkSmooth.AutoSize = true;
			this.checkSmooth.Checked = true;
			this.checkSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkSmooth.Location = new System.Drawing.Point(12, 427);
			this.checkSmooth.Name = "checkSmooth";
			this.checkSmooth.Size = new System.Drawing.Size(62, 17);
			this.checkSmooth.TabIndex = 46;
			this.checkSmooth.Text = "Smooth";
			this.checkSmooth.UseVisualStyleBackColor = true;
			// 
			// labelFPS
			// 
			this.labelFPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFPS.AutoSize = true;
			this.labelFPS.Location = new System.Drawing.Point(292, 503);
			this.labelFPS.Name = "labelFPS";
			this.labelFPS.Size = new System.Drawing.Size(33, 13);
			this.labelFPS.TabIndex = 58;
			this.labelFPS.Text = "FPS: ";
			// 
			// Form3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1023, 528);
			this.Controls.Add(this.labelFPS);
			this.Controls.Add(this.checkAxes);
			this.Controls.Add(this.checkPhong);
			this.Controls.Add(this.checkSpecular);
			this.Controls.Add(this.checkDiffuse);
			this.Controls.Add(this.checkAmbient);
			this.Controls.Add(this.checkShaders);
			this.Controls.Add(this.checkGlobalColor);
			this.Controls.Add(this.checkTexture);
			this.Controls.Add(this.checkVsync);
			this.Controls.Add(this.checkTwosided);
			this.Controls.Add(this.checkWireframe);
			this.Controls.Add(this.checkSmooth);
			this.Controls.Add(this.glControl1);
			this.Name = "Form3";
			this.Text = "Ray Visualiser";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
			this.ResumeLayout(false);
			this.PerformLayout();

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
	}
}