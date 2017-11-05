// Author: Josef Pelikan

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace _055quadtree
{
  class QuadTree
  {
    protected class QTNode
    {
      public QTNode ul, ur, ll, lr;

      public Color col;

      public QTNode ()
      {
        ul = ur = ll = lr = null;
      }

      public QTNode ( Color initCol )
      {
        ul = ur = ll = lr = null;
        col = initCol;
      }
    }

    #region protected data

    protected QTNode root = null;

    protected int width, height;

    #endregion

    #region constructor

    public QuadTree ()
    {
      root = null;
      width = height = 0;
    }

    #endregion

    #region Q-tree API

    public void EncodeTree ( Bitmap inp )
    {
      root = null;
      width = height = 0;
      if ( inp == null ) return;

      // !!!{{ TODO: add the Q-tree encoding code here

      width  = inp.Width;
      height = inp.Height;

      // !!!}}
    }

    public Bitmap DecodeTree ()
    {
      // !!!{{ TODO: add the Q-tree decoding here

      if ( width < 1 || height < 1 )
        return null;

      Bitmap result = new Bitmap( width, height, PixelFormat.Format24bppRgb );

      return result;

      // !!!}}
    }

    public void WriteTree ( Stream outs )
    {
      if ( outs == null ) return;

      // !!!{{ TODO: add the Q-tree writting code here

      StreamWriter w = new StreamWriter( outs );
      w.WriteLine( "QTREE {0} {1}", width, height );
      if ( root != null )
        WriteNode( w, root );

      w.Flush();

      // !!!}}
    }

    public bool ReadTree ( Stream ins )
    {
      root = null;
      width = height = 0;
      if ( ins == null ) return false;

      // !!!{{ TODO: add the Q-tree reading code here

      StreamReader r = new StreamReader( ins );
      string line = r.ReadLine();
      if ( line == null || line.Length < 1 )
        return false;

      string[] header = line.Split( ' ' );
      width = height = 0;

      if ( header.Length < 3 ||
           header[ 0 ] != "QTREE" ||
           !int.TryParse( header[ 1 ], out width ) ||
           !int.TryParse( header[ 2 ], out height ) )
        return false;

      if ( width > 0 && height > 0 )
        root = ReadNode( r );

      return true;

      // !!!}}
    }

    #endregion

    #region I/O support

    // !!!{{ You probably don't want to modify this code..?!

    protected void WriteNode ( StreamWriter w, QTNode n )
    {
      if ( n.ul == null )              // leaf node
        w.WriteLine( "{0} {1} {2}", n.col.R, n.col.G, n.col.B );
      else
      {
        w.WriteLine( 'R' );            // inner node
        WriteNode( w, n.ul );
        WriteNode( w, n.ur );
        WriteNode( w, n.ll );
        WriteNode( w, n.lr );
      }
    }

    protected QTNode ReadNode ( StreamReader r )
    {
      string ln = r.ReadLine();
      if ( ln == null ||
           ln.Length < 1 )
        return null;
      string[] line = ln.Split( ' ' );
      if ( line.Length < 1 ) return null;

      if ( line[ 0 ] == "R" )   // inner node
      {
        QTNode inner = new QTNode();
        inner.ul = ReadNode( r );
        inner.ur = ReadNode( r );
        inner.ll = ReadNode( r );
        inner.lr = ReadNode( r );
        return inner;
      }

      if ( line.Length < 3 ) return null;
      else                             // leaf node
      {
        int R, G, B;
        if ( !int.TryParse( line[ 0 ], out R ) ||
             !int.TryParse( line[ 1 ], out G ) ||
             !int.TryParse( line[ 2 ], out B ) )
          return null;

        return new QTNode( Color.FromArgb( R, G, B ) );
      }
    }

    // !!!}}

    #endregion
  }
}
