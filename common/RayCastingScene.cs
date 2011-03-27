using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Diagnostics;
using OpenTK;

// Interfaces and objects for ray-based rendering.
namespace Rendering
{
  /// <summary>
  /// Set operations for CSG inner nodes.
  /// </summary>
  public enum SetOperation
  {
    Union,
    Intersection,
    Difference,
    Xor,
  }

  /// <summary>
  /// Builtin
  /// </summary>
  public class PropertyName
  {

    /// <summary>
    /// Surface property = base color.
    /// </summary>
    const string COLOR = "color";

    /// <summary>
    /// Surface property = texture.
    /// </summary>
    const string TEXTURE = "texture";

    /// <summary>
    /// (Perhaps) globally used reflectance model.
    /// </summary>
    const string REFLECTANCE_MODEL = "reflectance";

    /// <summary>
    /// Surface property: material description. Must match used reflectance model.
    /// </summary>
    const string MATERIAL = "material";
  }

  /// <summary>
  /// General scene node (hierarchical 3D scene used in ray-based rendering).
  /// </summary>
  public interface ISceneNode : IIntersectable
  {
    /// <summary>
    /// Reference to a parent node (null for root node).
    /// </summary>
    ISceneNode Parent
    {
      get;
      set;
    }

    /// <summary>
    /// Transform from this space to parent's one.
    /// </summary>
    Matrix4d ToParent
    {
      get;
      set;
    }

    /// <summary>
    /// Transform from parent space to this node's space.
    /// </summary>
    Matrix4d FromParent
    {
      get;
      set;
    }

    /// <summary>
    /// Collection of node's children, can be null.
    /// </summary>
    ICollection<ISceneNode> Children
    {
      get;
      set;
    }

    /// <summary>
    /// Retrieves value of the given Attribute. Looks in parent nodes if not found locally.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <returns>Attribute value or null if not found.</returns>
    object GetAttribute ( string name );

    /// <summary>
    /// Sets the new value of the given attribute.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    void SetAttribute ( string name, object value );
  }

  /// <summary>
  /// Elementary solid - basic building block of a scene.
  /// </summary>
  public interface ISolid : ISceneNode
  {
    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    void CompleteIntersection ( Intersection inter );
  }

  /// <summary>
  /// Common code for ISceneNode.
  /// </summary>
  public class DefaultSceneNode : ISceneNode
  {
    protected LinkedList<ISceneNode> children;

    public ICollection<ISceneNode> Children
    {
      get
      {
        return children;
      }
      set
      {
        children.Clear();
        foreach ( ISceneNode sn in value )
          children.AddLast( sn );
      }
    }

    public ISceneNode Parent
    {
      get;
      set;
    }

    public Matrix4d ToParent
    {
      get;
      set;
    }

    public Matrix4d FromParent
    {
      get;
      set;
    }

    protected Dictionary< string, object > attributes;

    public object GetAttribute ( string name )
    {
      if ( attributes != null )
      {
        object result = attributes[ name ];
        if ( result != null ) return result;
      }
      if ( Parent != null ) return Parent.GetAttribute( name );
      return null;
    }

    public void SetAttribute ( string name, object value )
    {
      if ( attributes == null )
        attributes = new Dictionary< string, object >();
      attributes[ name ] = value;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public virtual LinkedList<Intersection> Intersect ( Vector4d p0, Vector3d p1 )
    {
      return null;
    }

    public DefaultSceneNode ()
    {
      children = new LinkedList<ISceneNode>();
      attributes = null;
    }
  }

  /// <summary>
  /// Inner CSG node (associated with a set operation).
  /// </summary>
  public class InnerNode : DefaultSceneNode
  {
    public SetOperation setOp;

    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector4d p0, Vector3d p1 )
    {
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      foreach ( ISceneNode n in children )
      {
        Vector4d origin = Vector4d.Transform( p0, n.FromParent );
      }
      return result;
    }

  }

}
