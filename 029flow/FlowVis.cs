// Author: Jan Dupej, Josef Pelikan

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MathSupport;

namespace _029flow
{
  public partial class Form1 : Form
  {
    /// <summary>
    /// Initialize the simulator object.
    /// </summary>
    private FluidSimulator getSimulator ( int ord )
    {
      FluidSimulator s = new FluidSimulator( progress, new RandomJames( ord * 13 ) );
      worldInitFunctions[ selectedWorld ]( s );
      return s;
    }

    /// <summary>
    /// Prepare combo-box of available scenes.
    /// </summary>
    private void InitializeScenes ()
    {
      worldInitFunctions = new List<InitWorldDelegate>( Worlds.InitFunctions );

      // 1. default worlds from Worlds
      foreach ( string name in Worlds.Names )
        comboScene.Items.Add( name );

      // 2. eventually add custom worlds
      //worldInitFunctions.Add( new InitWorldDelegate( xxx ) );
      //comboScene.Items.Add( "xxx" );

      // .. and set your favorite scene here:
      comboScene.SelectedIndex = comboScene.Items.IndexOf( "Roof" );

      // default visualization parameters?
      ImageWidth  = 600;
      ImageHeight = 200;
      checkMultithreading.Checked = true;
    }
  }
}
