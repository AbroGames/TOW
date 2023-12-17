using System.Linq;
using Godot;
using Godot.Collections;

namespace TOW.KludgeBox.Godot;

public static class GodotExtensions
{
    #region Vectors

    

    #endregion
    
    #region Objects/Nodes
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
    /// Поставит ноду последней в очередь на обработку (отрисовку). В результате она отрисуется поверх всех остальных нод.
    /// TODO: Подумать над переименованием в ToLast() или что-то такое
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
    /// Поставит ноду первой в очередь на обработку (отрисовку). В результате все остальные ноды отрисуются поверх неё.
    /// TODO: Подумать над переименованием в ToFirst() или что-то такое
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
    ///	Возвращает первую дочернюю ноду указанного типа, либо null, если ноды такого типа нет.
    /// </summary>
    public static T GetChild<T>(this Node node) where T : class
    {
        if (node.GetChildCount() < 1)
            return default;

        Array<Node> children = node.GetChildren();
        return children.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Пытается найти ближайшего родителя указанного типа. Тип также может быть интерфейсом.<br/>
    /// Вернет null, если такого родителя не встретилось вплоть до root.
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
    #endregion

    #region Color

    

    #endregion
}