namespace _016videoslow
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
      this.buttonDecode = new System.Windows.Forms.Button();
      this.buttonEncode = new System.Windows.Forms.Button();
      this.textInputMask = new System.Windows.Forms.TextBox();
      this.textOutputMask = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // buttonDecode
      // 
      this.buttonDecode.Location = new System.Drawing.Point( 369, 49 );
      this.buttonDecode.Name = "buttonDecode";
      this.buttonDecode.Size = new System.Drawing.Size( 97, 23 );
      this.buttonDecode.TabIndex = 5;
      this.buttonDecode.Text = "Decode";
      this.buttonDecode.UseVisualStyleBackColor = true;
      this.buttonDecode.Click += new System.EventHandler( this.buttonDecode_Click );
      // 
      // buttonEncode
      // 
      this.buttonEncode.Location = new System.Drawing.Point( 12, 49 );
      this.buttonEncode.Name = "buttonEncode";
      this.buttonEncode.Size = new System.Drawing.Size( 110, 23 );
      this.buttonEncode.TabIndex = 13;
      this.buttonEncode.Text = "Encode";
      this.buttonEncode.UseVisualStyleBackColor = true;
      this.buttonEncode.Click += new System.EventHandler( this.buttonEncode_Click );
      // 
      // textInputMask
      // 
      this.textInputMask.Location = new System.Drawing.Point( 13, 13 );
      this.textInputMask.Name = "textInputMask";
      this.textInputMask.Size = new System.Drawing.Size( 319, 20 );
      this.textInputMask.TabIndex = 14;
      this.textInputMask.Text = "input\\frame{0:0000}.png";
      // 
      // textOutputMask
      // 
      this.textOutputMask.Location = new System.Drawing.Point( 369, 14 );
      this.textOutputMask.Name = "textOutputMask";
      this.textOutputMask.Size = new System.Drawing.Size( 308, 20 );
      this.textOutputMask.TabIndex = 15;
      this.textOutputMask.Text = "output\\frame{0:0000}.png";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 712, 116 );
      this.Controls.Add( this.textOutputMask );
      this.Controls.Add( this.textInputMask );
      this.Controls.Add( this.buttonEncode );
      this.Controls.Add( this.buttonDecode );
      this.MinimumSize = new System.Drawing.Size( 720, 150 );
      this.Name = "Form1";
      this.Text = "016 video compress";
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonDecode;
    private System.Windows.Forms.Button buttonEncode;
    private System.Windows.Forms.TextBox textInputMask;
    private System.Windows.Forms.TextBox textOutputMask;
  }
}

