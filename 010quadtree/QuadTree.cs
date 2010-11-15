using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace _010quadtree
{
  class QuadTree
  {

    protected class QTNode
    {
      QTNode ul, ur, ll, lr;

      Color col;

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
      // !!!{{ TODO: add the Q-tree encoding code here


      // !!!}}
    }

    public Bitmap DecodeTree ()
    {
      // !!!{{ TODO: add the Q-tree decoding here

      return null;

      // !!!}}
    }

    #endregion

  }

}
