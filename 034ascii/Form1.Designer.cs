namespace _034ascii
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
          this.pictureBox1 = new System.Windows.Forms.PictureBox();
          this.btnOpen = new System.Windows.Forms.Button();
          this.label1 = new System.Windows.Forms.Label();
          this.txtWidth = new System.Windows.Forms.TextBox();
          this.txtHeight = new System.Windows.Forms.TextBox();
          this.label2 = new System.Windows.Forms.Label();
          this.btnConvert = new System.Windows.Forms.Button();
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
          this.SuspendLayout();
          // 
          // pictureBox1
          // 
          this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.pictureBox1.Location = new System.Drawing.Point( 13, 13 );
          this.pictureBox1.Name = "pictureBox1";
          this.pictureBox1.Size = new System.Drawing.Size( 677, 380 );
          this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
          this.pictureBox1.TabIndex = 3;
          this.pictureBox1.TabStop = false;
          // 
          // btnOpen
          // 
          this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.btnOpen.Location = new System.Drawing.Point( 12, 404 );
          this.btnOpen.Name = "btnOpen";
          this.btnOpen.Size = new System.Drawing.Size( 106, 29 );
          this.btnOpen.TabIndex = 4;
          this.btnOpen.Text = "Open image";
          this.btnOpen.UseVisualStyleBackColor = true;
          this.btnOpen.Click += new System.EventHandler( this.btnOpen_Click );
          // 
          // label1
          // 
          this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point( 147, 412 );
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size( 63, 13 );
          this.label1.TabIndex = 5;
          this.label1.Text = "Output size:";
          // 
          // txtWidth
          // 
          this.txtWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.txtWidth.Location = new System.Drawing.Point( 216, 409 );
          this.txtWidth.Name = "txtWidth";
          this.txtWidth.Size = new System.Drawing.Size( 44, 20 );
          this.txtWidth.TabIndex = 6;
          // 
          // txtHeight
          // 
          this.txtHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.txtHeight.Location = new System.Drawing.Point( 286, 409 );
          this.txtHeight.Name = "txtHeight";
          this.txtHeight.Size = new System.Drawing.Size( 44, 20 );
          this.txtHeight.TabIndex = 7;
          // 
          // label2
          // 
          this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.label2.AutoSize = true;
          this.label2.Location = new System.Drawing.Point( 268, 412 );
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size( 12, 13 );
          this.label2.TabIndex = 8;
          this.label2.Text = "x";
          // 
          // btnConvert
          // 
          this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
          this.btnConvert.Location = new System.Drawing.Point( 373, 404 );
          this.btnConvert.Name = "btnConvert";
          this.btnConvert.Size = new System.Drawing.Size( 109, 29 );
          this.btnConvert.TabIndex = 9;
          this.btnConvert.Text = "Engage!";
          this.btnConvert.UseVisualStyleBackColor = true;
          this.btnConvert.Click += new System.EventHandler( this.btnConvert_Click );
          // 
          // Form1
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size( 704, 442 );
          this.Controls.Add( this.btnConvert );
          this.Controls.Add( this.label2 );
          this.Controls.Add( this.txtHeight );
          this.Controls.Add( this.txtWidth );
          this.Controls.Add( this.label1 );
          this.Controls.Add( this.btnOpen );
          this.Controls.Add( this.pictureBox1 );
          this.MinimumSize = new System.Drawing.Size( 508, 140 );
          this.Name = "Form1";
          this.Text = "034 ascii";
          this.Load += new System.EventHandler( this.Form1_Load );
          ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
          this.ResumeLayout( false );
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConvert;
    }
}

