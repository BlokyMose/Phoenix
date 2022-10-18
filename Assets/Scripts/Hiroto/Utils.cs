using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // 移動可能な範囲
    public static Vector2 m_moveLimit = new Vector2(8f, 4.5f);

    // 指定された位置を移動可能な範囲に収めた値を返す
    public static Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3
        (
            Mathf.Clamp(position.x, -m_moveLimit.x, m_moveLimit.x),
            Mathf.Clamp(position.y, -m_moveLimit.y, m_moveLimit.y),
            0
        );
    }

    public static float GetAngle(Vector2 from, Vector2 to)
    {
        var dx = to.x - from.x;
        var dy = to.y - from.y;
        var rad = Mathf.Atan2(dy, dx);
        return rad * Mathf.Rad2Deg;
    }

    internal static object GetDirection(float angle)
    {
        throw new System.NotImplementedException();
    }
}