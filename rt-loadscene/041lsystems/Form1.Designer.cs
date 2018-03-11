namespace _041lsystems
{
  partial class LSystemMainWin
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
        components.Dispose();
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
      this.glCanvas = new OpenTK.GLControl();
      this.resetButton = new System.Windows.Forms.Button();
      this.saveButton = new System.Windows.Forms.Button();
      this.loadButton = new System.Windows.Forms.Button();
      this.label11 = new System.Windows.Forms.Label();
      this.shortage = new System.Windows.Forms.TrackBar();
      this.label10 = new System.Windows.Forms.Label();
      this.generationStep = new System.Windows.Forms.TrackBar();
      this.label9 = new System.Windows.Forms.Label();
      this.shrinkage = new System.Windows.Forms.TrackBar();
      this.label8 = new System.Windows.Forms.Label();
      this.length = new System.Windows.Forms.TrackBar();
      this.label7 = new System.Windows.Forms.Label();
      this.angle = new System.Windows.Forms.TrackBar();
      this.label6 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.generateButton = new System.Windows.Forms.Button();
      this.iterationCount = new System.Windows.Forms.NumericUpDown();
      this.resultList = new System.Windows.Forms.ListBox();
      this.startSymbol = new System.Windows.Forms.ComboBox();
      this.textBox3 = new System.Windows.Forms.TextBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.removeRuleButton = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.ruleLeftSideCombo = new System.Windows.Forms.ComboBox();
      this.addNewRuleButton = new System.Windows.Forms.Button();
      this.ruleRightSide = new System.Windows.Forms.TextBox();
      this.ruleWeight = new System.Windows.Forms.NumericUpDown();
      this.ruleList = new System.Windows.Forms.ListBox();
      this.changeRuleButton = new System.Windows.Forms.Button();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.shortage)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.generationStep)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.shrinkage)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.length)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.angle)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.iterationCount)).BeginInit();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ruleWeight)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point( 0, 0 );
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add( this.glCanvas );
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.AutoScroll = true;
      this.splitContainer1.Panel2.AutoScrollMinSize = new System.Drawing.Size( 260, 0 );
      this.splitContainer1.Panel2.Controls.Add( this.resetButton );
      this.splitContainer1.Panel2.Controls.Add( this.saveButton );
      this.splitContainer1.Panel2.Controls.Add( this.loadButton );
      this.splitContainer1.Panel2.Controls.Add( this.label11 );
      this.splitContainer1.Panel2.Controls.Add( this.shortage );
      this.splitContainer1.Panel2.Controls.Add( this.label10 );
      this.splitContainer1.Panel2.Controls.Add( this.generationStep );
      this.splitContainer1.Panel2.Controls.Add( this.label9 );
      this.splitContainer1.Panel2.Controls.Add( this.shrinkage );
      this.splitContainer1.Panel2.Controls.Add( this.label8 );
      this.splitContainer1.Panel2.Controls.Add( this.length );
      this.splitContainer1.Panel2.Controls.Add( this.label7 );
      this.splitContainer1.Panel2.Controls.Add( this.angle );
      this.splitContainer1.Panel2.Controls.Add( this.label6 );
      this.splitContainer1.Panel2.Controls.Add( this.label2 );
      this.splitContainer1.Panel2.Controls.Add( this.label1 );
      this.splitContainer1.Panel2.Controls.Add( this.generateButton );
      this.splitContainer1.Panel2.Controls.Add( this.iterationCount );
      this.splitContainer1.Panel2.Controls.Add( this.resultList );
      this.splitContainer1.Panel2.Controls.Add( this.startSymbol );
      this.splitContainer1.Panel2.Controls.Add( this.textBox3 );
      this.splitContainer1.Panel2.Controls.Add( this.panel1 );
      this.splitContainer1.Panel2.Controls.Add( this.ruleList );
      this.splitContainer1.Size = new System.Drawing.Size( 980, 784 );
      this.splitContainer1.SplitterDistance = 698;
      this.splitContainer1.TabIndex = 0;
      // 
      // glCanvas
      // 
      this.glCanvas.BackColor = System.Drawing.Color.Black;
      this.glCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
      this.glCanvas.Location = new System.Drawing.Point( 0, 0 );
      this.glCanvas.Name = "glCanvas";
      this.glCanvas.Size = new System.Drawing.Size( 696, 782 );
      this.glCanvas.TabIndex = 0;
      this.glCanvas.VSync = false;
      this.glCanvas.Load += new System.EventHandler( this.glControl1_Load );
      this.glCanvas.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.glCanvas_MouseWheel );
      this.glCanvas.Paint += new System.Windows.Forms.PaintEventHandler( this.glControl1_Paint );
      this.glCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseMove );
      this.glCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseDown );
      this.glCanvas.Resize += new System.EventHandler( this.glControl1_Resize );
      this.glCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler( this.glControl1_MouseUp );
      // 
      // resetButton
      // 
      this.resetButton.Location = new System.Drawing.Point( 6, 3 );
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size( 78, 23 );
      this.resetButton.TabIndex = 26;
      this.resetButton.Text = "Reset";
      this.resetButton.UseVisualStyleBackColor = true;
      this.resetButton.Click += new System.EventHandler( this.resetButton_Click );
      // 
      // saveButton
      // 
      this.saveButton.Location = new System.Drawing.Point( 187, 3 );
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size( 83, 23 );
      this.saveButton.TabIndex = 25;
      this.saveButton.Text = "Save Rules ...";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler( this.saveButton_Click );
      // 
      // loadButton
      // 
      this.loadButton.Location = new System.Drawing.Point( 90, 3 );
      this.loadButton.Name = "loadButton";
      this.loadButton.Size = new System.Drawing.Size( 91, 23 );
      this.loadButton.TabIndex = 24;
      this.loadButton.Text = "Load Rules ...";
      this.loadButton.UseVisualStyleBackColor = true;
      this.loadButton.Click += new System.EventHandler( this.loadButton_Click );
      // 
      // label11
      // 
      this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point( 7, 635 );
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size( 50, 13 );
      this.label11.TabIndex = 23;
      this.label11.Text = "Shortage";
      // 
      // shortage
      // 
      this.shortage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.shortage.Location = new System.Drawing.Point( 70, 621 );
      this.shortage.Maximum = 200;
      this.shortage.Name = "shortage";
      this.shortage.Size = new System.Drawing.Size( 203, 45 );
      this.shortage.TabIndex = 22;
      this.shortage.TickFrequency = 5;
      this.shortage.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.shortage.Value = 90;
      this.shortage.ValueChanged += new System.EventHandler( this.Redraw );
      // 
      // label10
      // 
      this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point( 10, 693 );
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size( 29, 13 );
      this.label10.TabIndex = 21;
      this.label10.Text = "Step";
      // 
      // generationStep
      // 
      this.generationStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.generationStep.Location = new System.Drawing.Point( 74, 675 );
      this.generationStep.Maximum = 100;
      this.generationStep.Minimum = 1;
      this.generationStep.Name = "generationStep";
      this.generationStep.Size = new System.Drawing.Size( 198, 45 );
      this.generationStep.TabIndex = 20;
      this.generationStep.TickFrequency = 5;
      this.generationStep.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.generationStep.Value = 20;
      this.generationStep.ValueChanged += new System.EventHandler( this.Redraw );
      // 
      // label9
      // 
      this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point( 10, 741 );
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size( 55, 13 );
      this.label9.TabIndex = 19;
      this.label9.Text = "Shrinkage";
      // 
      // shrinkage
      // 
      this.shrinkage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.shrinkage.Location = new System.Drawing.Point( 71, 726 );
      this.shrinkage.Maximum = 200;
      this.shrinkage.Name = "shrinkage";
      this.shrinkage.Size = new System.Drawing.Size( 203, 45 );
      this.shrinkage.TabIndex = 18;
      this.shrinkage.TickFrequency = 5;
      this.shrinkage.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.shrinkage.Value = 70;
      this.shrinkage.ValueChanged += new System.EventHandler( this.Redraw );
      // 
      // label8
      // 
      this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point( 10, 584 );
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size( 40, 13 );
      this.label8.TabIndex = 17;
      this.label8.Text = "Length";
      // 
      // length
      // 
      this.length.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.length.Location = new System.Drawing.Point( 70, 570 );
      this.length.Maximum = 100;
      this.length.Name = "length";
      this.length.Size = new System.Drawing.Size( 203, 45 );
      this.length.TabIndex = 16;
      this.length.TickFrequency = 5;
      this.length.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.length.Value = 30;
      this.length.ValueChanged += new System.EventHandler( this.Redraw );
      // 
      // label7
      // 
      this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point( 10, 534 );
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size( 34, 13 );
      this.label7.TabIndex = 15;
      this.label7.Text = "Angle";
      // 
      // angle
      // 
      this.angle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.angle.Location = new System.Drawing.Point( 70, 519 );
      this.angle.Maximum = 180;
      this.angle.Minimum = -180;
      this.angle.Name = "angle";
      this.angle.Size = new System.Drawing.Size( 203, 45 );
      this.angle.TabIndex = 14;
      this.angle.TickFrequency = 15;
      this.angle.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.angle.Value = 25;
      this.angle.ValueChanged += new System.EventHandler( this.Redraw );
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point( 5, 355 );
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size( 79, 13 );
      this.label6.TabIndex = 12;
      this.label6.Text = "Iteration Count:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 10, 63 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 29, 13 );
      this.label2.TabIndex = 11;
      this.label2.Text = "Start";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 10, 37 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 50, 13 );
      this.label1.TabIndex = 10;
      this.label1.Text = "Variables";
      // 
      // generateButton
      // 
      this.generateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.generateButton.Location = new System.Drawing.Point( 155, 350 );
      this.generateButton.Name = "generateButton";
      this.generateButton.Size = new System.Drawing.Size( 116, 23 );
      this.generateButton.TabIndex = 9;
      this.generateButton.Text = "Generate";
      this.generateButton.UseVisualStyleBackColor = true;
      this.generateButton.Click += new System.EventHandler( this.generateButton_Click );
      // 
      // iterationCount
      // 
      this.iterationCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.iterationCount.Location = new System.Drawing.Point( 90, 351 );
      this.iterationCount.Maximum = new decimal( new int[] {
            50,
            0,
            0,
            0} );
      this.iterationCount.Minimum = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      this.iterationCount.Name = "iterationCount";
      this.iterationCount.Size = new System.Drawing.Size( 51, 20 );
      this.iterationCount.TabIndex = 8;
      this.iterationCount.Value = new decimal( new int[] {
            5,
            0,
            0,
            0} );
      // 
      // resultList
      // 
      this.resultList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.resultList.FormattingEnabled = true;
      this.resultList.HorizontalScrollbar = true;
      this.resultList.Location = new System.Drawing.Point( 3, 379 );
      this.resultList.Name = "resultList";
      this.resultList.Size = new System.Drawing.Size( 270, 134 );
      this.resultList.TabIndex = 7;
      this.resultList.SelectedIndexChanged += new System.EventHandler( this.Redraw );
      // 
      // startSymbol
      // 
      this.startSymbol.FormattingEnabled = true;
      this.startSymbol.Location = new System.Drawing.Point( 82, 60 );
      this.startSymbol.Name = "startSymbol";
      this.startSymbol.Size = new System.Drawing.Size( 59, 21 );
      this.startSymbol.TabIndex = 6;
      this.startSymbol.SelectedIndexChanged += new System.EventHandler( this.comboBox1_SelectedIndexChanged );
      // 
      // textBox3
      // 
      this.textBox3.Location = new System.Drawing.Point( 82, 34 );
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new System.Drawing.Size( 188, 20 );
      this.textBox3.TabIndex = 5;
      this.textBox3.TextChanged += new System.EventHandler( this.textBox3_TextChanged );
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.Controls.Add( this.changeRuleButton );
      this.panel1.Controls.Add( this.removeRuleButton );
      this.panel1.Controls.Add( this.label5 );
      this.panel1.Controls.Add( this.label4 );
      this.panel1.Controls.Add( this.label3 );
      this.panel1.Controls.Add( this.ruleLeftSideCombo );
      this.panel1.Controls.Add( this.addNewRuleButton );
      this.panel1.Controls.Add( this.ruleRightSide );
      this.panel1.Controls.Add( this.ruleWeight );
      this.panel1.Location = new System.Drawing.Point( 3, 240 );
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size( 270, 104 );
      this.panel1.TabIndex = 4;
      // 
      // removeRuleButton
      // 
      this.removeRuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.removeRuleButton.Location = new System.Drawing.Point( 152, 76 );
      this.removeRuleButton.Name = "removeRuleButton";
      this.removeRuleButton.Size = new System.Drawing.Size( 115, 23 );
      this.removeRuleButton.TabIndex = 9;
      this.removeRuleButton.Text = "Remove Rule";
      this.removeRuleButton.UseVisualStyleBackColor = true;
      this.removeRuleButton.Click += new System.EventHandler( this.removeRuleButton_Click );
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point( 197, 4 );
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size( 32, 13 );
      this.label5.TabIndex = 8;
      this.label5.Text = "Right";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point( 104, 3 );
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size( 41, 13 );
      this.label4.TabIndex = 7;
      this.label4.Text = "Weight";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 37, 4 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 25, 13 );
      this.label3.TabIndex = 6;
      this.label3.Text = "Left";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // ruleLeftSideCombo
      // 
      this.ruleLeftSideCombo.FormattingEnabled = true;
      this.ruleLeftSideCombo.Location = new System.Drawing.Point( 3, 20 );
      this.ruleLeftSideCombo.Name = "ruleLeftSideCombo";
      this.ruleLeftSideCombo.Size = new System.Drawing.Size( 88, 21 );
      this.ruleLeftSideCombo.TabIndex = 5;
      // 
      // addNewRuleButton
      // 
      this.addNewRuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.addNewRuleButton.Location = new System.Drawing.Point( 5, 47 );
      this.addNewRuleButton.Name = "addNewRuleButton";
      this.addNewRuleButton.Size = new System.Drawing.Size( 262, 23 );
      this.addNewRuleButton.TabIndex = 4;
      this.addNewRuleButton.Text = "Add Rule";
      this.addNewRuleButton.UseVisualStyleBackColor = true;
      this.addNewRuleButton.Click += new System.EventHandler( this.addNewRuleButton_Click );
      // 
      // ruleRightSide
      // 
      this.ruleRightSide.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ruleRightSide.Location = new System.Drawing.Point( 154, 20 );
      this.ruleRightSide.Name = "ruleRightSide";
      this.ruleRightSide.Size = new System.Drawing.Size( 111, 20 );
      this.ruleRightSide.TabIndex = 3;
      // 
      // ruleWeight
      // 
      this.ruleWeight.DecimalPlaces = 3;
      this.ruleWeight.Increment = new decimal( new int[] {
            10,
            0,
            0,
            131072} );
      this.ruleWeight.Location = new System.Drawing.Point( 93, 20 );
      this.ruleWeight.Maximum = new decimal( new int[] {
            10,
            0,
            0,
            0} );
      this.ruleWeight.Name = "ruleWeight";
      this.ruleWeight.Size = new System.Drawing.Size( 59, 20 );
      this.ruleWeight.TabIndex = 2;
      this.ruleWeight.Value = new decimal( new int[] {
            1,
            0,
            0,
            0} );
      // 
      // ruleList
      // 
      this.ruleList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ruleList.FormattingEnabled = true;
      this.ruleList.Location = new System.Drawing.Point( 3, 87 );
      this.ruleList.Name = "ruleList";
      this.ruleList.Size = new System.Drawing.Size( 270, 147 );
      this.ruleList.TabIndex = 0;
      this.ruleList.SelectedIndexChanged += new System.EventHandler( this.ruleList_SelectedIndexChanged );
      // 
      // changeRuleButton
      // 
      this.changeRuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.changeRuleButton.Location = new System.Drawing.Point( 7, 76 );
      this.changeRuleButton.Name = "changeRuleButton";
      this.changeRuleButton.Size = new System.Drawing.Size( 131, 23 );
      this.changeRuleButton.TabIndex = 10;
      this.changeRuleButton.Text = "Change Rule";
      this.changeRuleButton.UseVisualStyleBackColor = true;
      this.changeRuleButton.Click += new System.EventHandler( this.changeRuleButton_Click );
      // 
      // LSystemMainWin
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 980, 784 );
      this.Controls.Add( this.splitContainer1 );
      this.MinimumSize = new System.Drawing.Size( 980, 750 );
      this.Name = "LSystemMainWin";
      this.Text = "041 L-systems";
      this.splitContainer1.Panel1.ResumeLayout( false );
      this.splitContainer1.Panel2.ResumeLayout( false );
      this.splitContainer1.Panel2.PerformLayout();
      this.splitContainer1.ResumeLayout( false );
      ((System.ComponentModel.ISupportInitialize)(this.shortage)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.generationStep)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.shrinkage)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.length)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.angle)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.iterationCount)).EndInit();
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ruleWeight)).EndInit();
      this.ResumeLayout( false );

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.ListBox ruleList;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button addNewRuleButton;
    private System.Windows.Forms.TextBox ruleRightSide;
    private System.Windows.Forms.NumericUpDown ruleWeight;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.ComboBox ruleLeftSideCombo;
    private System.Windows.Forms.ComboBox startSymbol;
    private System.Windows.Forms.ListBox resultList;
    private System.Windows.Forms.Button generateButton;
    private System.Windows.Forms.NumericUpDown iterationCount;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label6;
    private OpenTK.GLControl glCanvas;
    private System.Windows.Forms.TrackBar angle;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TrackBar length;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TrackBar shrinkage;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TrackBar generationStep;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.TrackBar shortage;
    private System.Windows.Forms.Button removeRuleButton;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.Button loadButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.Button changeRuleButton;
  }
}
