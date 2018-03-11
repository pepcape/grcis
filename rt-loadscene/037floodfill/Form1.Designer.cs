namespace _037floodfill
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
      this.numericXstart = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.numericGranul = new System.Windows.Forms.NumericUpDown();
      this.labelQueue = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.numericYstart = new System.Windows.Forms.NumericUpDown();
      this.buttonStop = new System.Windows.Forms.Button();
      this.buttonStart = new System.Windows.Forms.Button();
      this.checkSnap = new System.Windows.Forms.CheckBox();
      this.buttonLoad = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericXstart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericGranul)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericYstart)).BeginInit();
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
      this.panel1.Size = new System.Drawing.Size( 680, 380 );
      this.panel1.TabIndex = 0;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point( 0, 0 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 680, 380 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      // 
      // numericXstart
      // 
      this.numericXstart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericXstart.Location = new System.Drawing.Point( 289, 406 );
      this.numericXstart.Maximum = new decimal( new int[] {
            10000,
            0,
            0,
            0} );
      this.numericXstart.Name = "numericXstart";
      this.numericXstart.Size = new System.Drawing.Size( 74, 20 );
      this.numericXstart.TabIndex = 3;
      this.numericXstart.Value = new decimal( new int[] {
            50,
            0,
            0,
            0} );
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 246, 408 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 37, 13 );
      this.label1.TabIndex = 4;
      this.label1.Text = "Xstart:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 71, 442 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 60, 13 );
      this.label2.TabIndex = 6;
      this.label2.Text = "Granularity:";
      // 
      // numericGranul
      // 
      this.numericGranul.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericGranul.Increment = new decimal( new int[] {
            100,
            0,
            0,
            0} );
      this.numericGranul.Location = new System.Drawing.Point( 141, 439 );
      this.numericGranul.Maximum = new decimal( new int[] {
            100000,
            0,
            0,
            0} );
      this.numericGranul.Minimum = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.numericGranul.Name = "numericGranul";
      this.numericGranul.Size = new System.Drawing.Size( 87, 20 );
      this.numericGranul.TabIndex = 7;
      this.numericGranul.Value = new decimal( new int[] {
            1000,
            0,
            0,
            0} );
      // 
      // labelQueue
      // 
      this.labelQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.labelQueue.AutoSize = true;
      this.labelQueue.Location = new System.Drawing.Point( 517, 442 );
      this.labelQueue.Name = "labelQueue";
      this.labelQueue.Size = new System.Drawing.Size( 42, 13 );
      this.labelQueue.TabIndex = 8;
      this.labelQueue.Text = "Queue:";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 246, 443 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 37, 13 );
      this.label3.TabIndex = 10;
      this.label3.Text = "Ystart:";
      // 
      // numericYstart
      // 
      this.numericYstart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.numericYstart.Location = new System.Drawing.Point( 289, 439 );
      this.numericYstart.Maximum = new decimal( new int[] {
            10000,
            0,
            0,
            0} );
      this.numericYstart.Name = "numericYstart";
      this.numericYstart.Size = new System.Drawing.Size( 74, 20 );
      this.numericYstart.TabIndex = 11;
      this.numericYstart.Value = new decimal( new int[] {
            50,
            0,
            0,
            0} );
      // 
      // buttonStop
      // 
      this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point( 397, 436 );
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size( 97, 23 );
      this.buttonStop.TabIndex = 12;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler( this.buttonStop_Click );
      // 
      // buttonStart
      // 
      this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonStart.Enabled = false;
      this.buttonStart.Location = new System.Drawing.Point( 397, 405 );
      this.buttonStart.Name = "buttonStart";
      this.buttonStart.Size = new System.Drawing.Size( 97, 23 );
      this.buttonStart.TabIndex = 13;
      this.buttonStart.Text = "Fill";
      this.buttonStart.UseVisualStyleBackColor = true;
      this.buttonStart.Click += new System.EventHandler( this.buttonStart_Click );
      // 
      // checkSnap
      // 
      this.checkSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkSnap.AutoSize = true;
      this.checkSnap.Location = new System.Drawing.Point( 520, 409 );
      this.checkSnap.Name = "checkSnap";
      this.checkSnap.Size = new System.Drawing.Size( 108, 17 );
      this.checkSnap.TabIndex = 14;
      this.checkSnap.Text = "Save snapshots?";
      this.checkSnap.UseVisualStyleBackColor = true;
      // 
      // buttonLoad
      // 
      this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonLoad.Location = new System.Drawing.Point( 13, 405 );
      this.buttonLoad.Name = "buttonLoad";
      this.buttonLoad.Size = new System.Drawing.Size( 215, 23 );
      this.buttonLoad.TabIndex = 15;
      this.buttonLoad.Text = "Load image";
      this.buttonLoad.UseVisualStyleBackColor = true;
      this.buttonLoad.Click += new System.EventHandler( this.buttonLoad_Click );
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 474 );
      this.Controls.Add( this.buttonLoad );
      this.Controls.Add( this.checkSnap );
      this.Controls.Add( this.buttonStart );
      this.Controls.Add( this.buttonStop );
      this.Controls.Add( this.numericYstart );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.labelQueue );
      this.Controls.Add( this.numericGranul );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.numericXstart );
      this.Controls.Add( this.panel1 );
      this.MinimumSize = new System.Drawing.Size( 700, 200 );
      this.Name = "Form1";
      this.Text = "037 flood-fill";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.Form1_FormClosing );
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericXstart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericGranul)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericYstart)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.NumericUpDown numericXstart;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numericGranul;
    private System.Windows.Forms.Label labelQueue;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numericYstart;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.Button buttonStart;
    private System.Windows.Forms.CheckBox checkSnap;
    private System.Windows.Forms.Button buttonLoad;
  }
}

