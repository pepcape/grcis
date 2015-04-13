namespace _029flow
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
      this.buttonSave = new System.Windows.Forms.Button();
      this.buttonSimulation = new System.Windows.Forms.Button();
      this.labelElapsed = new System.Windows.Forms.Label();
      this.labelSample = new System.Windows.Forms.Label();
      this.buttonStop = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.comboScene = new System.Windows.Forms.ComboBox();
      this.checkMultithreading = new System.Windows.Forms.CheckBox();
      this.buttonRes = new System.Windows.Forms.Button();
      this.checkPressure = new System.Windows.Forms.CheckBox();
      this.buttonResults = new System.Windows.Forms.Button();
      this.checkVelocity = new System.Windows.Forms.CheckBox();
      this.checkCustom = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textParam = new System.Windows.Forms.TextBox();
      this.buttonLoad = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.AutoScroll = true;
      this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add( this.pictureBox1 );
      this.panel1.Location = new System.Drawing.Point( 13, 13 );
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size( 680, 360 );
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point( 0, 0 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 680, 360 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler( this.pictureBox1_MouseDown );
      this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler( this.pictureBox1_MouseMove );
      // 
      // buttonSave
      // 
      this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonSave.Location = new System.Drawing.Point( 602, 447 );
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size( 91, 23 );
      this.buttonSave.TabIndex = 2;
      this.buttonSave.Text = "Save image";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler( this.buttonSave_Click );
      // 
      // buttonSimulation
      // 
      this.buttonSimulation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonSimulation.Location = new System.Drawing.Point( 13, 447 );
      this.buttonSimulation.Name = "buttonSimulation";
      this.buttonSimulation.Size = new System.Drawing.Size( 125, 23 );
      this.buttonSimulation.TabIndex = 5;
      this.buttonSimulation.Text = "[Re]start simulation";
      this.buttonSimulation.UseVisualStyleBackColor = true;
      this.buttonSimulation.Click += new System.EventHandler( this.buttonSimulation_Click );
      // 
      // labelElapsed
      // 
      this.labelElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelElapsed.AutoSize = true;
      this.labelElapsed.Location = new System.Drawing.Point( 199, 453 );
      this.labelElapsed.Name = "labelElapsed";
      this.labelElapsed.Size = new System.Drawing.Size( 48, 13 );
      this.labelElapsed.TabIndex = 21;
      this.labelElapsed.Text = "Elapsed:";
      // 
      // labelSample
      // 
      this.labelSample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelSample.AutoSize = true;
      this.labelSample.Location = new System.Drawing.Point( 331, 389 );
      this.labelSample.Name = "labelSample";
      this.labelSample.Size = new System.Drawing.Size( 35, 13 );
      this.labelSample.TabIndex = 22;
      this.labelSample.Text = "State:";
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point( 144, 447 );
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size( 43, 23 );
      this.buttonStop.TabIndex = 32;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler( this.buttonStop_Click );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 12, 388 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 38, 13 );
      this.label1.TabIndex = 33;
      this.label1.Text = "World:";
      // 
      // comboScene
      // 
      this.comboScene.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboScene.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboScene.FormattingEnabled = true;
      this.comboScene.Location = new System.Drawing.Point( 59, 384 );
      this.comboScene.Name = "comboScene";
      this.comboScene.Size = new System.Drawing.Size( 140, 21 );
      this.comboScene.TabIndex = 34;
      this.comboScene.SelectedIndexChanged += new System.EventHandler( this.comboScene_SelectedIndexChanged );
      // 
      // checkMultithreading
      // 
      this.checkMultithreading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkMultithreading.AutoSize = true;
      this.checkMultithreading.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkMultithreading.Location = new System.Drawing.Point( 13, 418 );
      this.checkMultithreading.Name = "checkMultithreading";
      this.checkMultithreading.Size = new System.Drawing.Size( 94, 17 );
      this.checkMultithreading.TabIndex = 41;
      this.checkMultithreading.Text = "multi-threading";
      this.checkMultithreading.UseVisualStyleBackColor = true;
      // 
      // buttonRes
      // 
      this.buttonRes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRes.Location = new System.Drawing.Point( 218, 383 );
      this.buttonRes.Name = "buttonRes";
      this.buttonRes.Size = new System.Drawing.Size( 101, 23 );
      this.buttonRes.TabIndex = 42;
      this.buttonRes.Text = "Resolution";
      this.buttonRes.UseVisualStyleBackColor = true;
      this.buttonRes.Click += new System.EventHandler( this.buttonRes_Click );
      // 
      // checkPressure
      // 
      this.checkPressure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkPressure.AutoSize = true;
      this.checkPressure.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkPressure.Location = new System.Drawing.Point( 202, 418 );
      this.checkPressure.Name = "checkPressure";
      this.checkPressure.Size = new System.Drawing.Size( 66, 17 );
      this.checkPressure.TabIndex = 43;
      this.checkPressure.Text = "pressure";
      this.checkPressure.UseVisualStyleBackColor = true;
      this.checkPressure.CheckedChanged += new System.EventHandler( this.checkPressure_CheckedChanged );
      // 
      // buttonResults
      // 
      this.buttonResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonResults.Location = new System.Drawing.Point( 602, 418 );
      this.buttonResults.Name = "buttonResults";
      this.buttonResults.Size = new System.Drawing.Size( 91, 23 );
      this.buttonResults.TabIndex = 44;
      this.buttonResults.Text = "Save results";
      this.buttonResults.UseVisualStyleBackColor = true;
      this.buttonResults.Click += new System.EventHandler( this.buttonResults_Click );
      // 
      // checkVelocity
      // 
      this.checkVelocity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkVelocity.AutoSize = true;
      this.checkVelocity.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkVelocity.Checked = true;
      this.checkVelocity.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkVelocity.Location = new System.Drawing.Point( 124, 418 );
      this.checkVelocity.Name = "checkVelocity";
      this.checkVelocity.Size = new System.Drawing.Size( 62, 17 );
      this.checkVelocity.TabIndex = 45;
      this.checkVelocity.Text = "velocity";
      this.checkVelocity.UseVisualStyleBackColor = true;
      this.checkVelocity.CheckedChanged += new System.EventHandler( this.checkVelocity_CheckedChanged );
      // 
      // checkCustom
      // 
      this.checkCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkCustom.AutoSize = true;
      this.checkCustom.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkCustom.Location = new System.Drawing.Point( 288, 418 );
      this.checkCustom.Name = "checkCustom";
      this.checkCustom.Size = new System.Drawing.Size( 60, 17 );
      this.checkCustom.TabIndex = 46;
      this.checkCustom.Text = "custom";
      this.checkCustom.UseVisualStyleBackColor = true;
      this.checkCustom.CheckedChanged += new System.EventHandler( this.checkCustom_CheckedChanged );
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 366, 419 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 40, 13 );
      this.label2.TabIndex = 47;
      this.label2.Text = "Param:";
      // 
      // textParam
      // 
      this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.textParam.Location = new System.Drawing.Point( 412, 418 );
      this.textParam.Name = "textParam";
      this.textParam.Size = new System.Drawing.Size( 175, 20 );
      this.textParam.TabIndex = 48;
      this.textParam.TextChanged += new System.EventHandler( this.textParam_TextChanged );
      // 
      // buttonLoad
      // 
      this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonLoad.Location = new System.Drawing.Point( 505, 448 );
      this.buttonLoad.Name = "buttonLoad";
      this.buttonLoad.Size = new System.Drawing.Size( 91, 23 );
      this.buttonLoad.TabIndex = 49;
      this.buttonLoad.Text = "Load results";
      this.buttonLoad.UseVisualStyleBackColor = true;
      this.buttonLoad.Click += new System.EventHandler( this.buttonLoad_Click );
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 482 );
      this.Controls.Add( this.buttonLoad );
      this.Controls.Add( this.textParam );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.checkCustom );
      this.Controls.Add( this.checkVelocity );
      this.Controls.Add( this.buttonResults );
      this.Controls.Add( this.checkPressure );
      this.Controls.Add( this.buttonRes );
      this.Controls.Add( this.checkMultithreading );
      this.Controls.Add( this.comboScene );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.buttonStop );
      this.Controls.Add( this.labelSample );
      this.Controls.Add( this.labelElapsed );
      this.Controls.Add( this.buttonSimulation );
      this.Controls.Add( this.buttonSave );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 720, 300 );
      this.Name = "Form1";
      this.Text = "029 Flow simulator";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.Form1_FormClosing );
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Button buttonSimulation;
    private System.Windows.Forms.Label labelElapsed;
    private System.Windows.Forms.Label labelSample;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox comboScene;
    private System.Windows.Forms.CheckBox checkMultithreading;
    private System.Windows.Forms.Button buttonRes;
    private System.Windows.Forms.CheckBox checkPressure;
    private System.Windows.Forms.Button buttonResults;
    private System.Windows.Forms.CheckBox checkVelocity;
    private System.Windows.Forms.CheckBox checkCustom;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textParam;
    private System.Windows.Forms.Button buttonLoad;
  }
}

