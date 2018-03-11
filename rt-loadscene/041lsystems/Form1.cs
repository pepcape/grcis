using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace _041lsystems
{
  public partial class LSystemMainWin : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    public LSystemMainWin ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';

      mLSystem = new LSystem();
      mLSystemGenerator = new LSystemGenerator();
      mLSystemRenderer = new LSystemRenderer();
    }

    private void textBox3_TextChanged ( object sender, EventArgs e )
    {
      HashSet<char> alphabet = new HashSet<char>();
      foreach ( char i in textBox3.Text )
        alphabet.Add( i );
      startSymbol.Items.Clear();
      ruleLeftSideCombo.Items.Clear();

      mAlphabet = new List<char>( alphabet );
      mAlphabet.Sort();

      foreach ( char i in mAlphabet )
      {
        startSymbol.Items.Add( i );
        ruleLeftSideCombo.Items.Add( i );
      }
    }

    private void generateButton_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;

      mLSystemGenerator.Generate( mLSystem, (int)iterationCount.Value, out iterations );
      resultList.Items.Clear();
      foreach ( string i in iterations )
        resultList.Items.Add( (i.Length < 7600) ? i : i.Substring( 0, 7580 ) + " <truncated>" );

      Cursor.Current = Cursors.Default;
    }

    private void addNewRuleButton_Click ( object sender, EventArgs e )
    {
      if ( ruleLeftSideCombo.Text == null || ruleLeftSideCombo.Text.Length == 0 )
      {
        MessageBox.Show( "Left side of the rule must not be empty!" );
        return;
      }

      if ( !mLSystem.AddRule( ruleLeftSideCombo.Text[ 0 ], (float)ruleWeight.Value, ruleRightSide.Text ) )
        MessageBox.Show( "Duplicate rule, ignored." );
      else
        FillRules();
    }

    private void ruleList_SelectedIndexChanged ( object sender, EventArgs e )
    {
      LSystemRule.RuleRightSide rule = (LSystemRule.RuleRightSide)ruleList.SelectedItem;
      if ( rule != null )
      {
        ruleLeftSideCombo.Text = rule.left.ToString();
        ruleWeight.Value = (decimal)rule.weight;
        ruleRightSide.Text = rule.rule;
      }
    }

    private void changeRuleButton_Click ( object sender, EventArgs e )
    {
      LSystemRule.RuleRightSide rule = (LSystemRule.RuleRightSide)ruleList.SelectedItem;
      if ( rule != null && ruleLeftSideCombo.Text != null && ruleLeftSideCombo.Text.Length > 0 )
      {
        mLSystem.ChangeRule( rule, ruleLeftSideCombo.Text[ 0 ], (float)ruleWeight.Value, ruleRightSide.Text );
        FillRules();
      }
    }

    private void removeRuleButton_Click ( object sender, EventArgs e )
    {
      LSystemRule.RuleRightSide rule = (LSystemRule.RuleRightSide)ruleList.SelectedItem;
      if ( rule != null )
      {
        ruleList.Items.Remove( rule );
        mLSystem.RemoveRule( rule );
      }
    }

    private void FillRules ()
    {
      ruleList.BeginUpdate();
      ruleList.Items.Clear();
      foreach ( KeyValuePair<char, LSystemRule> ruleRec in mLSystem.rules )
      {
        LSystemRule rule = ruleRec.Value;
        foreach ( LSystemRule.RuleRightSide rside in rule.mRightSides )
          ruleList.Items.Add( rside );
      }
      ruleList.EndUpdate();
      mLSystem.ToString();
    }

    protected List<char> mAlphabet;
    protected Dictionary<char, LSystemRule> mRules;
    protected List<string> iterations;
    protected LSystemGenerator mLSystemGenerator;
    protected LSystem mLSystem;
    protected LSystemRenderer mLSystemRenderer;

    private void comboBox1_SelectedIndexChanged ( object sender, EventArgs e )
    {
      mLSystem.start = startSymbol.Text[ 0 ];
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      VisualisationParameters param = new VisualisationParameters();
      param.angle = angle.Value;
      param.length = length.Value * 0.1;
      param.shortage = shortage.Value * 0.01;
      param.shrinkage = shrinkage.Value * 0.01;
      int index = resultList.SelectedIndex;
      param.radius = System.Math.Max( 0.03, index * generationStep.Value * 0.01 );

      string toInterpret = (iterations != null && index >= 0 && index < iterations.Count) ? iterations[ index ] : "";
      mLSystemRenderer.Render( toInterpret, index, param );

      glCanvas.SwapBuffers();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      SetupViewport();

      GL.Enable( EnableCap.Lighting );
      GL.Enable( EnableCap.ColorMaterial );
      GL.ColorMaterial( MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse );
      GL.Enable( EnableCap.Light0 );
      GL.Light( LightName.Light0, LightParameter.Diffuse, new Color4( 1.0f, 1.0f, 1.0f, 1.0f ) );

      GL.Enable( EnableCap.Light1 );
      GL.Light( LightName.Light1, LightParameter.Position, new Vector4( 50.0f, 50.0f, 150.0f, 1.0f ) );
    }

    private void SetupViewport ()
    {
      int wid = glCanvas.Width;
      int hei = glCanvas.Height;

      // 1. set ViewPort transform:
      GL.Viewport( 0, 0, wid, hei );

      // 2. set projection matrix
      GL.MatrixMode( MatrixMode.Projection );
      Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView( 1.0f, wid / (float)hei, 0.1f, 200.0f );
      GL.LoadMatrix( ref proj );
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      SetupViewport();
      glCanvas.Invalidate();
    }

    private void Redraw ( object sender, EventArgs e )
    {
      glCanvas.Invalidate();
    }

    private void glControl1_MouseDown ( object sender, MouseEventArgs e )
    {
      mOrbit = true;
      mClickLocation = e.Location;
    }

    private void glControl1_MouseUp ( object sender, MouseEventArgs e )
    {
      mOrbit = false;
    }

    private bool mOrbit = false;
    private Point mClickLocation;

    private void glControl1_MouseMove ( object sender, MouseEventArgs e )
    {
      if ( !mOrbit ) return;

      Point p = new Point();
      p.X = mClickLocation.X - e.Location.X;
      p.Y = mClickLocation.Y - e.Location.Y;
      mClickLocation = e.Location;
      double factor = -5.0 / (double)glCanvas.Width;

      mLSystemRenderer.OrbitCamera( p.X * factor, p.Y * factor );
      glCanvas.Invalidate();
    }

    private void loadButton_Click ( object sender, EventArgs e )
    {
      OpenFileDialog openDialog = new OpenFileDialog();
      openDialog.Title = "Open L-system rule file";
      openDialog.Filter = "L-system rule files|*.xml" +
                          "|All files|*.*";
      openDialog.FilterIndex = 1;
      openDialog.FileName = "";
      if ( openDialog.ShowDialog() != DialogResult.OK )
        return;

      if ( !mLSystem.LoadFromFile( openDialog.FileName ) )
      {
        MessageBox.Show( this, "Loading failed" );
        return;
      }
      FillRules();
      textBox3.Text = mLSystem.GetVariables();
      startSymbol.SelectedItem = mLSystem.start;
    }

    private void saveButton_Click ( object sender, EventArgs e )
    {
      SaveFileDialog saveDialog = new SaveFileDialog();
      saveDialog.Title = "Save L-system rule file";
      saveDialog.Filter = "L-system rule files|*.xml";
      saveDialog.AddExtension = true;
      saveDialog.FileName = "";
      if ( saveDialog.ShowDialog() != DialogResult.OK )
        return;

      if ( !mLSystem.SaveToFile( saveDialog.FileName ) )
        MessageBox.Show( this, "Saving failed" );
    }

    private void resetButton_Click ( object sender, EventArgs e )
    {
      mLSystem.Reset();
      FillRules();
    }

    private void glCanvas_MouseWheel ( object sender, System.Windows.Forms.MouseEventArgs e )
    {
      mLSystemRenderer.Zoom( Math.Pow( 1.05, -1.0 * ((double)e.Delta / 120.0) ) );
      glCanvas.Invalidate();
    }
  }
}
