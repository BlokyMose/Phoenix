using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encore.Utility
{
    public static class MathUtility
    {
        public struct Line
        {
            public Vector2 p1;
            public Vector2 p2;

            public Line(Vector2 p1, Vector2 p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }
        public static Vector2? GetIntersection(Line lineA, Line lineB)
        {
            var x =
                ((lineA.p1.x * lineA.p2.y - lineA.p1.y * lineA.p2.x) * (lineB.p1.x - lineB.p2.x) - (lineA.p1.x - lineA.p2.x) * (lineB.p1.x * lineB.p2.y - lineB.p1.y * lineB.p2.x))
                /
                ((lineA.p1.x - lineA.p2.x) * (lineB.p1.y - lineB.p2.y) - (lineA.p1.y - lineA.p2.y) * (lineB.p1.x - lineB.p2.x));
            
            if (float.IsNaN(x))
                return null;

            var y =
                ((lineA.p1.x * lineA.p2.y - lineA.p1.y * lineA.p2.x) * (lineB.p1.y - lineB.p2.y) - (lineA.p1.y - lineA.p2.y) * (lineB.p1.x * lineB.p2.y - lineB.p1.y * lineB.p2.x))
                /
                ((lineA.p1.x - lineA.p2.x) * (lineB.p1.y - lineB.p2.y) - (lineA.p1.y - lineA.p2.y) * (lineB.p1.x - lineB.p2.x));

            if (float.IsNaN(y))
                return null;

            return new Vector2(x, y);
        }
    }
}
