using System.Linq;
using Godot;
using Godot.Collections;

namespace TOW.Scripts.KludgeBox.Godot;

public static class NodeExtensions
{
    /// <summary>
    /// Shortcut for GodotObject.IsInstanceValid(object)
    /// </summary>
    /// <param name="gdObj"></param>
    /// <returns></returns>
    public static bool IsValid(this GodotObject gdObj)
    {
        return GodotObject.IsInstanceValid(gdObj);
    }
    
    /// <summary>
    /// Calculates the distance between two Node2D objects.
    /// </summary>
    /// <param name="node">The first Node2D object.</param>
    /// <param name="other">The second Node2D object.</param>
    /// <returns>The distance between the two Node2D objects.</returns>
    public static double DistanceTo(this Node2D node, Node2D other)
    {
        return node.Position.DistanceTo(other.Position);
    }

    /// <summary>
    /// Calculates the distance between a Node2D object and a Vector2 position.
    /// </summary>
    /// <param name="node">The Node2D object.</param>
    /// <param name="pos">The Vector2 position.</param>
    /// <returns>The distance between the Node2D object and the Vector2 position.</returns>
    public static double DistanceTo(this Node2D node, Vector2 pos)
    {
        return node.Position.DistanceTo(pos);
    }

    /// <summary>
    /// Calculates the distance between a Vector2 position and a Node2D object.
    /// </summary>
    /// <param name="pos">The Vector2 position.</param>
    /// <param name="other">The Node2D object.</param>
    /// <returns>The distance between the Vector2 position and the Node2D object.</returns>
    public static double DistanceTo(this Vector2 pos, Node2D other)
    {
        return pos.DistanceTo(other.Position);
    }
    
    /// <summary>
    /// Sets the node as last in the queue for processing (rendering). As a result, it will be drawn on top of all other nodes.
    /// </summary>
    public static void ToForeground(this Node node)
    {
        var parent = node.GetParent();

        if (GodotObject.IsInstanceValid(parent))
        {
            parent.MoveChild(node, -1);
        }
    }

    /// <summary>
    /// Sets the node as first in the queue for processing (rendering). As a result, all other nodes will be drawn on top of it.
    /// </summary>
    public static void ToBackground(this Node node)
    {
        var parent = node.GetParent();

        if (GodotObject.IsInstanceValid(parent))
        {
            parent.MoveChild(node, 0);
        }
    }
    
    /// <summary>
    /// Returns the first child node of the specified type or null if there is no node of that type.
    /// </summary>
    public static T GetChild<T>(this Node node) where T : class
    {
        if (node.GetChildCount() < 1)
            return default;

        Array<Node> children = node.GetChildren();
        return children.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Attempts to find the nearest parent of the specified type. The type can also be an interface.<br/>
    /// Returns null if such a parent is not encountered up to the root.
    /// </summary>
    public static T GetParent<T>(this Node child) where T : class
    {
        var parent = child.GetParent();
        if (parent is null)
            return default;

        if (parent is T)
            return parent as T;

        return parent.GetParent<T>();
    }
}