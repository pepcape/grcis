namespace _040morph3d
{
  partial class MorphForm
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
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.glControl2 = new OpenTK.GLControl();
      this.glControl3 = new OpenTK.GLControl();
      this.glControl1 = new OpenTK.GLControl();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.buttonLoadFirst = new System.Windows.Forms.Button();
      this.buttonLoadSecond = new System.Windows.Forms.Button();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point( 0, 0 );
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add( this.splitContainer2 );
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add( this.glControl1 );
      this.splitContainer1.Size = new System.Drawing.Size( 810, 471 );
      this.splitContainer1.SplitterDistance = 337;
      this.splitContainer1.TabIndex = 0;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point( 0, 0 );
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add( this.glControl2 );
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add( this.glControl3 );
      this.splitContainer2.Size = new System.Drawing.Size( 337, 471 );
      this.splitContainer2.SplitterDistance = 227;
      this.splitContainer2.TabIndex = 0;
      // 
      // glControl2
      // 
      this.glControl2.BackColor = System.Drawing.Color.Black;
      this.glControl2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.glControl2.Location = new System.Drawing.Point( 0, 0 );
      this.glControl2.Name = "glControl2";
      this.glControl2.Size = new System.Drawing.Size( 337, 227 );
      this.glControl2.TabIndex = 1;
      this.glControl2.VSync = false;
      this.glControl2.Load += new System.EventHandler( this.glControl_Load );
      this.glControl2.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseWheel );
      this.glControl2.MouseLeave += new System.EventHandler( this.glControl_MouseLeave );
      this.glControl2.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl_Paint );
      this.glControl2.MouseMove += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseMove );
      this.glControl2.MouseDown += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseDown );
      this.glControl2.Resize += new System.EventHandler( this.glControl_Resize );
      this.glControl2.MouseUp += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseUp );
      // 
      // glControl3
      // 
      this.glControl3.BackColor = System.Drawing.Color.Black;
      this.glControl3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.glControl3.Location = new System.Drawing.Point( 0, 0 );
      this.glControl3.Name = "glControl3";
      this.glControl3.Size = new System.Drawing.Size( 337, 240 );
      this.glControl3.TabIndex = 1;
      this.glControl3.VSync = false;
      this.glControl3.Load += new System.EventHandler( this.glControl_Load );
      this.glControl3.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseWheel );
      this.glControl3.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl_Paint );
      this.glControl3.MouseMove += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseMove );
      this.glControl3.MouseDown += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseDown );
      this.glControl3.Resize += new System.EventHandler( this.glControl_Resize );
      this.glControl3.MouseUp += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseUp );
      // 
      // glControl1
      // 
      this.glControl1.BackColor = System.Drawing.Color.Black;
      this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.glControl1.Location = new System.Drawing.Point( 0, 0 );
      this.glControl1.Name = "glControl1";
      this.glControl1.Size = new System.Drawing.Size( 469, 471 );
      this.glControl1.TabIndex = 0;
      this.glControl1.VSync = false;
      this.glControl1.Load += new System.EventHandler( this.glControl_Load );
      this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseWheel );
      this.glControl1.MouseLeave += new System.EventHandler( this.glControl_MouseLeave );
      this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl_Paint );
      this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseMove );
      this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseDown );
      this.glControl1.Resize += new System.EventHandler( this.glControl_Resize );
      this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler( this.glControl_MouseUp );
      // 
      // trackBar1
      // 
      this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.trackBar1.Location = new System.Drawing.Point( 341, 514 );
      this.trackBar1.Maximum = 100;
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size( 457, 45 );
      this.trackBar1.TabIndex = 1;
      this.trackBar1.TickFrequency = 5;
      this.trackBar1.ValueChanged += new System.EventHandler( this.trackBar1_ValueChanged );
      // 
      // buttonLoadFirst
      // 
      this.buttonLoadFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoadFirst.Location = new System.Drawing.Point( 22, 488 );
      this.buttonLoadFirst.Name = "buttonLoadFirst";
      this.buttonLoadFirst.Size = new System.Drawing.Size( 99, 23 );
      this.buttonLoadFirst.TabIndex = 2;
      this.buttonLoadFirst.Text = "Load first ...";
      this.buttonLoadFirst.UseVisualStyleBackColor = true;
      this.buttonLoadFirst.Click += new System.EventHandler( this.loadButtonPressed );
      // 
      // buttonLoadSecond
      // 
      this.buttonLoadSecond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoadSecond.Location = new System.Drawing.Point( 22, 526 );
      this.buttonLoadSecond.Name = "buttonLoadSecond";
      this.buttonLoadSecond.Size = new System.Drawing.Size( 99, 23 );
      this.buttonLoadSecond.TabIndex = 3;
      this.buttonLoadSecond.Text = "Load second ...";
      this.buttonLoadSecond.UseVisualStyleBackColor = true;
      this.buttonLoadSecond.Click += new System.EventHandler( this.loadButtonPressed );
      // 
      // MorphForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 810, 571 );
      this.Controls.Add( this.buttonLoadSecond );
      this.Controls.Add( this.buttonLoadFirst );
      this.Controls.Add( this.trackBar1 );
      this.Controls.Add( this.splitContainer1 );
      this.Name = "MorphForm";
      this.Text = "040 3D morphing ";
      this.splitContainer1.Panel1.ResumeLayout( false );
      this.splitContainer1.Panel2.ResumeLayout( false );
      this.splitContainer1.ResumeLayout( false );
      this.splitContainer2.Panel1.ResumeLayout( false );
      this.splitContainer2.Panel2.ResumeLayout( false );
      this.splitContainer2.ResumeLayout( false );
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private OpenTK.GLControl glControl1;
    private OpenTK.GLControl glControl2;
    private OpenTK.GLControl glControl3;
    private System.Windows.Forms.TrackBar trackBar1;
    private System.Windows.Forms.Button buttonLoadFirst;
    private System.Windows.Forms.Button buttonLoadSecond;
  }
}

