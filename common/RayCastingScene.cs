using System.Collections.Generic;
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
    public static string COLOR = "color";

    /// <summary>
    /// Surface property = texture (multi-attribute).
    /// </summary>
    public static string TEXTURE = "texture";

    /// <summary>
    /// (Perhaps) globally used reflectance model.
    /// </summary>
    public static string REFLECTANCE_MODEL = "reflectance";

    /// <summary>
    /// Surface property: material description. Must match used reflectance model.
    /// </summary>
    public static string MATERIAL = "material";
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
    /// True for object root (subject to animation). 3D texture coordinates should use Object space.
    /// </summary>
    bool ObjectRoot
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
    /// Retrieves value of the given Attribute. Looks only in this node.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <returns>Attribute value or null if not found.</returns>
    object GetLocalAttribute ( string name );

    /// <summary>
    /// Sets the new value of the given attribute.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Attribute value.</param>
    void SetAttribute ( string name, object value );

    /// <summary>
    /// Returns transform from the Local space (Solid) to the World space.
    /// </summary>
    /// <returns>Transform matrix.</returns>
    Matrix4d ToWorld ();

    /// <summary>
    /// Returns transform from the Local space (Solid) to the Object space (subject to animation).
    /// </summary>
    /// <returns>Transform matrix.</returns>
    Matrix4d ToObject ();

    /// <summary>
    /// Collects texture sequence in the right (application) order.
    /// </summary>
    /// <returns>Sequence of textures or null.</returns>
    LinkedList<ITexture> GetTextures ();
  }

  /// <summary>
  /// Elementary solid - atomic building block of a scene.
  /// </summary>
  public interface ISolid : ISceneNode
  {
  }

  /// <summary>
  /// Common code for ISceneNode.
  /// </summary>
  public abstract class DefaultSceneNode : ISceneNode
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

    public bool ObjectRoot
    {
      get;
      set;
    }

    protected Dictionary<string, object> attributes;

    public object GetAttribute ( string name )
    {
      if ( attributes != null )
      {
        object result;
        if ( attributes.TryGetValue( name, out result ) )
          return result;
      }
      if ( Parent != null ) return Parent.GetAttribute( name );
      return null;
    }

    public object GetLocalAttribute ( string name )
    {
      object result;
      if ( attributes == null ||
           !attributes.TryGetValue( name, out result ) )
        return null;

      return result;
    }

    public void SetAttribute ( string name, object value )
    {
      if ( attributes == null )
        attributes = new Dictionary< string, object >();
      attributes[ name ] = value;
    }

    /// <summary>
    /// Returns transform from the Local space (Solid) to the World space.
    /// </summary>
    /// <returns>Transform matrix.</returns>
    public Matrix4d ToWorld ()
    {
      if ( Parent == null ) return Matrix4d.Identity;
      return( ToParent * Parent.ToWorld() );
    }

    /// <summary>
    /// Returns transform from the Local space (Solid) to the Object space (subject to animation).
    /// </summary>
    /// <returns>Transform matrix.</returns>
    public Matrix4d ToObject ()
    {
      if ( ObjectRoot || Parent == null ) return Matrix4d.Identity;
      return( ToParent * Parent.ToObject() );
    }

    /// <summary>
    /// Collects texture sequence in the right (application) order.
    /// </summary>
    /// <returns>Sequence of textures or null.</returns>
    public LinkedList<ITexture> GetTextures ()
    {
      LinkedList<ITexture> result = null;
      if ( Parent != null )
        result = Parent.GetTextures();

      object local = GetLocalAttribute( PropertyName.TEXTURE );
      if ( local == null ) return result;

      if ( local is ITexture )
      {
        if ( result == null )
          result = new LinkedList<ITexture>();
        result.AddLast( (ITexture)local );
      }
      else
        if ( local is IEnumerable<ITexture> )
          if ( result == null )
            result = new LinkedList<ITexture>( (IEnumerable<ITexture>)local );
          else
            foreach ( ITexture tex in (IEnumerable<ITexture>)local )
              result.AddLast( tex );

      return result;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public virtual LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      return null;
    }

    /// <summary>
    /// Complete all relevant items in the given Intersection object.
    /// </summary>
    /// <param name="inter">Intersection instance to complete.</param>
    public virtual void CompleteIntersection ( Intersection inter )
    { }

    public DefaultSceneNode ()
    {
      children = new LinkedList<ISceneNode>();
      attributes = null;
      ObjectRoot = false;
    }
  }

  /// <summary>
  /// Inner CSG node (associated with a set operation).
  /// </summary>
  public class InnerNode : DefaultSceneNode
  {
    public SetOperation setOp;

    /// <summary>
    /// Creates an empty inner scene node.
    /// </summary>
    /// <param name="op">Set operation to use.</param>
    public InnerNode ( SetOperation op )
    {
      setOp = op;
    }

    /// <summary>
    /// Inserts one new child node to this parent node.
    /// </summary>
    /// <param name="ch">Child node to add.</param>
    /// <param name="toParent">Transform from local space of the child to the parent's space.</param>
    public void InsertChild ( ISceneNode ch, Matrix4d toParent )
    {
      children.AddLast( ch );
      ch.ToParent   = toParent;
      toParent.Invert();
      ch.FromParent = toParent;
      ch.Parent     = this;
    }

    /// <summary>
    /// Computes the complete intersection of the given ray with the object. 
    /// </summary>
    /// <param name="p0">Ray origin.</param>
    /// <param name="p1">Ray direction vector.</param>
    /// <returns>Sorted list of intersection records.</returns>
    public override LinkedList<Intersection> Intersect ( Vector3d p0, Vector3d p1 )
    {
      LinkedList<Intersection> result = new LinkedList<Intersection>();
      foreach ( ISceneNode n in children )
      {
        Vector3d origin = Vector3d.TransformPosition( p0, n.FromParent );
        Vector3d dir    = Vector3d.TransformVector(   p1, n.FromParent );
        // ray in local child's coords: [ origin, dir ]

        LinkedList<Intersection> partial = n.Intersect( origin, dir );
        if ( partial == null || partial.Count == 0 ) continue;

        Intersection i = null;
        foreach ( Intersection inter in partial )
          if ( inter.T > 0.0 )
          {
            i = inter;
            break;
          }

        if ( i != null )
          if ( result.First == null )
            result.AddFirst( i );
          else
            if ( i.T < result.First.Value.T )
              result.AddFirst( i );
      }
      return result;
    }

  }
}
